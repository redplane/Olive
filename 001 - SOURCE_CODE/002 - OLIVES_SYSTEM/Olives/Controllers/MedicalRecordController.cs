using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Newtonsoft.Json;
using Olives.Attributes;
using Olives.ViewModels.Edit;
using Olives.ViewModels.Initialize;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Filter;

namespace Olives.Controllers
{
    [Route("api/medical/record")]
    public class MedicalRecordController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        /// <param name="repositoryMedical"></param>
        /// <param name="repositoryRelation"></param>
        /// <param name="timeService"></param>
        /// <param name="log"></param>
        public MedicalRecordController(IRepositoryAccount repositoryAccount, IRepositoryMedicalRecord repositoryMedical,
            IRepositoryRelation repositoryRelation, ITimeService timeService,
            ILog log)
        {
            _repositoryAccount = repositoryAccount;
            _repositoryMedical = repositoryMedical;
            _repositoryRelation = repositoryRelation;
            _timeService = timeService;
            _log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find a specialty by using specialty id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> Get([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find the medical record by using id.
            var medicalRecord = await _repositoryMedical.FindMedicalRecordAsync(id);

            // No result has been received.
            if (medicalRecord == null)
            {
                // Log the error.
                _log.Error($"Medical record [Id: {id}] is not found.");

                // Tell client no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Requester doesn't take part in medical record.
            if (requester.Id != medicalRecord.Owner)
            {
                // Find the relationship between the requester and owner.
                var relationships =
                    await
                        _repositoryRelation.FindRelationshipAsync(requester.Id, medicalRecord.Owner,
                            (byte) StatusRelation.Active);

                if (relationships == null || relationships.Count < 1)
                {
                    // Log the error.
                    _log.Error(
                        $"There is no relationship between Requester[Id: {requester.Id}] and Medical Record Owner [Id: {medicalRecord.Owner}]");

                    // Tell client no record has been found.
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                MedicalRecord = new
                {
                    medicalRecord.Id,
                    medicalRecord.Owner,
                    medicalRecord.Creator,
                    Category = new
                    {
                        medicalRecord.MedicalCategory.Id,
                        medicalRecord.MedicalCategory.Name
                    },
                    medicalRecord.Info,
                    medicalRecord.Time,
                    medicalRecord.Created,
                    medicalRecord.LastModified
                }
            });
        }

        /// <summary>
        ///     Add a medical record asyncrhonously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> Post([FromBody] InitializeMedicalRecordViewModel info)
        {
            #region Parameters validate

            // Model hasn't been initialized.
            if (info == null)
            {
                // Initialize it and do the validation.
                info = new InitializeMedicalRecordViewModel();
                Validate(info);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Error sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Owner validate

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Owner who owns the medical record.
            Person owner = null;

            // No owner is specified.
            if (info.Owner == null)
            {
                // Let the owner be requester as it hasn't been defined.
                info.Owner = requester.Id;
                owner = requester;
            }
            else
            {
                if (info.Owner == requester.Id)
                    owner = requester;
                else
                    owner =
                        await
                            _repositoryAccount.FindPersonAsync(info.Owner, null, null, (byte) Role.Patient,
                                StatusAccount.Active);
            }

            // Owner is not found.
            if (owner == null)
            {
                // Log the error.
                _log.Error($"Person (owner) [Id: {info.Owner}] is not found.");

                // Tell the client that the action is forbidden.
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnOwnerNotActive}"
                });
            }

            #endregion

            #region Requester validate

            // Requester is a doctor.
            if (requester.Id == info.Owner.Value)
            {
                // Doctor cannot create medical record for him/herself.
                if (requester.Role == (byte) Role.Doctor)
                {
                    // Log the error.
                    _log.Error($"Doctor [Id: {requester.Id}] cannot create medical record for him/herself");

                    // Tell the client about error.
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnMedicalRecordDoctorToDoctor}"
                    });
                }
            }
            else
            {
                // Find the relationship between requester and the record owner.
                var relationship =
                    await _repositoryRelation.FindRelationshipAsync(requester.Id, info.Owner.Value,
                        (byte) StatusRelation.Active);

                // No relationship is found between 2 people.
                if (relationship == null || relationship.Count < 1)
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Error = $"{Language.WarnHasNoRelationship}"
                    });
            }

            #endregion

            #region Record initialization

            try
            {
                // Only filter and receive the first result.
                var medicalRecord = new MedicalRecord();
                medicalRecord.Owner = info.Owner.Value;
                medicalRecord.Category = info.Category;
                medicalRecord.Info = JsonConvert.SerializeObject(info.Infos);
                medicalRecord.Time = info.Time;
                medicalRecord.Created = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

                // Insert a new allergy to database.
                var result = await _repositoryMedical.InitializeMedicalRecordAsync(medicalRecord);

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    MedicalRecord = new
                    {
                        result.Id,
                        result.Owner,
                        result.Creator,
                        Category = new
                        {
                            result.MedicalCategory.Id,
                            result.MedicalCategory.Name
                        },
                        result.Info,
                        result.Time,
                        result.Created
                    }
                });
            }
            catch (Exception exception)
            {
                // Log the exception first.
                _log.Error(exception.Message, exception);

                // Tell the client something is wrong with the server.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        /// <summary>
        ///     Edit an addiction asynchronously.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [HttpPut]
        [OlivesAuthorize(new[] {Role.Doctor})]
        public async Task<HttpResponseMessage> Put([FromUri] int id, [FromBody] EditMedicalRecordViewModel info)
        {
            #region Paramters validation

            // Model hasn't been initialized.
            if (info == null)
            {
                // Initialize it and do the validation.
                info = new EditMedicalRecordViewModel();
                Validate(info);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Request parameters are invalid. Error sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Medical record validation.

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find the record first.
            var medicalRecord = await _repositoryMedical.FindMedicalRecordAsync(id);

            // Medical record is not found.
            if (medicalRecord == null)
            {
                // Log the error.
                _log.Error($"Medical record [Id: {id}] is not found");

                // Tell the client that medical record is not found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            #endregion

            #region Relationship validation

            // Check the relationship between them.
            var relationship = await _repositoryRelation.FindRelationshipAsync(requester.Id, medicalRecord.Owner,
                (byte) StatusAccount.Active);
            if (relationship == null || relationship.Count < 1)
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = $"{Language.WarnHasNoRelationship}"
                });

            #endregion

            #region Information update

            try
            {
                // Infos needs updating.
                if (info.Infos != null)
                    medicalRecord.Info = JsonConvert.SerializeObject(info.Infos);

                // Time needs updating.
                if (info.Time != null)
                    medicalRecord.Time = info.Time.Value;

                // Update the last time
                medicalRecord.LastModified = _timeService.DateTimeUtcToUnix(DateTime.UtcNow);

                // Insert a new allergy to database.
                var result = await _repositoryMedical.InitializeMedicalRecordAsync(medicalRecord);

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    MedicalRecord = new
                    {
                        result.Id,
                        result.Info,
                        Category = new
                        {
                            result.MedicalCategory.Id,
                            result.MedicalCategory.Name
                        },
                        result.Time,
                        result.Created,
                        result.LastModified
                    }
                });
            }
            catch (Exception exception)
            {
                // As the exception occurs, log it first.
                _log.Error(exception.Message, exception);

                // Tell the client something goes wrong with the server.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        /// <summary>
        ///     Filter medical record by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/medical/record/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Patient, Role.Doctor})]
        public async Task<HttpResponseMessage> FilterMedicalRecord([FromBody] FilterMedicalRecordViewModel filter)
        {
            #region Paramters validation

            // Model hasn't been initialized.
            if (filter == null)
            {
                // Initialize it and do the validation.
                filter = new FilterMedicalRecordViewModel();
                Validate(filter);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid medical record request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Information filter

            try
            {
                // Retrieve information of person who sent request.
                var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

                // Update the requester.
                filter.Requester = requester.Id;

                // Filter medical records.
                var results = await _repositoryMedical.FilterMedicalRecordAsync(filter);

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    MedicalRecords = results.MedicalRecords.Select(x => new
                    {
                        x.Id,
                        x.Owner,
                        x.Creator,
                        Category = new
                        {
                            x.MedicalCategory.Id,
                            x.MedicalCategory.Name
                        },
                        x.Info,
                        x.Time,
                        x.Created,
                        x.LastModified
                    }),
                    results.Total
                });
            }
            catch (Exception exception)
            {
                // As exception happens, log the error first.
                _log.Error(exception.Message, exception);

                // Tell the client something goes wrong with the server.
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }

            #endregion
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository of accounts
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

        /// <summary>
        ///     Repository of allergies
        /// </summary>
        private readonly IRepositoryMedicalRecord _repositoryMedical;

        /// <summary>
        ///     Repository of relationships.
        /// </summary>
        private readonly IRepositoryRelation _repositoryRelation;

        /// <summary>
        ///     Service which provides functions to access time calculation.
        /// </summary>
        private readonly ITimeService _timeService;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}