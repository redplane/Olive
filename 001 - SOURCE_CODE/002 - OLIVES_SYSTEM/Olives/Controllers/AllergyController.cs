using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Interfaces;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Helpers;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Filter;
using Shared.ViewModels.Initialize;

namespace Olives.Controllers
{
    public class AllergyController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of SpecialtyController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        /// <param name="repositoryAllergy"></param>
        /// <param name="log"></param>
        /// <param name="emailService"></param>
        public AllergyController(IRepositoryAccount repositoryAccount, IRepositoryAllergy repositoryAllergy, ILog log,
            IEmailService emailService)
        {
            _repositoryAccount = repositoryAccount;
            _repositoryAllergy = repositoryAllergy;
            _log = log;
            _emailService = emailService;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Find a specialty by using specialty id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/allergy")]
        [HttpGet]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> Get([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Only filter and receive the first result.
            var filter = new FilterAllergyViewModel();
            filter.Id = id;
            filter.Owner = requester.Id;
            filter.Page = 0;
            filter.Records = 1;

            // Retrieve the results list.
            var results = await _repositoryAllergy.FilterAllergy(filter);

            // No result has been received.
            if (results == null || results.Allergies == null || results.Allergies.Count != 1)
            {
                // Tell client no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Retrieve the 1st queried result.
            var result = results.Allergies
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Cause,
                    x.Note,
                    x.Created,
                    x.LastModified
                })
                .FirstOrDefault();

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Allergy = result
            });
        }

        /// <summary>
        ///     Insert an allergy asyncrhonously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/allergy")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> Post([FromBody] InitializeAllergyViewModel info)
        {
            #region ModelState result

            // Model hasn't been initialized.
            if (info == null)
            {
                // Initialize the request view model and do the validation.
                info = new InitializeAllergyViewModel();
                Validate(info);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid allergies filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Only filter and receive the first result.
            var allergy = new Allergy();
            allergy.Owner = requester.Id;
            allergy.Name = info.Name;
            allergy.Cause = info.Cause;
            allergy.Note = info.Note;
            allergy.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);

            // Insert a new allergy to database.
            var result = await _repositoryAllergy.InitializeAllergyAsync(allergy);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Allergy = new
                {
                    result.Id,
                    result.Name,
                    result.Cause,
                    result.Note,
                    result.Created
                }
            });
        }

        /// <summary>
        ///     Edit an allergy.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/allergy")]
        [HttpPut]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> Put([FromUri] int id, [FromBody] InitializeAllergyViewModel info)
        {
            #region ModelState result

            // Model hasn't been initialized.
            if (info == null)
            {
                _log.Error("Invalid allergies filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    Errors = new[] {Language.InvalidRequestParameters}
                });
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid allergies filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find allergy by using allergy id and owner id.
            var allergies = await _repositoryAllergy.FindAllergyAsync(id, requester.Id);

            // Not record has been found.
            if (allergies == null || allergies.Count != 1)
            {
                // Tell client no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Retrieve the first record.
            var allergy = allergies.FirstOrDefault();
            if (allergy == null)
            {
                // Tell client no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }

            // Confirm edit.
            allergy.Name = info.Name;
            allergy.Cause = info.Cause;
            allergy.Note = info.Note;
            allergy.LastModified = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.Now);

            // Update allergy.
            allergy = await _repositoryAllergy.InitializeAllergyAsync(allergy);

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Allergy = new
                {
                    allergy.Id,
                    allergy.Name,
                    allergy.Cause,
                    allergy.Note,
                    allergy.Created,
                    allergy.LastModified
                }
            });
        }

        /// <summary>
        ///     Delete an allergy.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/allergy")]
        [HttpDelete]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> Delete([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            try
            {
                // Find and delete the allergy.
                var deletedRecords = await _repositoryAllergy.DeleteAllergyAsync(id, requester.Id);

                // No record has been deleted.
                if (deletedRecords < 1)
                {
                    // Tell front-end, no record has been found.
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Errors = new[] {Language.WarnRecordNotFound}
                    });
                }

                // Tell client the deletion is ok.
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                // Write the error to file.
                _log.Error(exception.Message, exception);

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Error = $"{Language.WarnInternalServerError}"
                });
            }
        }

        /// <summary>
        ///     Filter specialties by using specific conditions.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/allergy/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Doctor, Role.Patient})]
        public async Task<HttpResponseMessage> Filter([FromBody] FilterAllergyViewModel info)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Model hasn't been initialized.
            if (info == null)
            {
                _log.Error("Invalid allergies filter request parameters");
                info = new FilterAllergyViewModel();
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid allergies filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            // Owner has been specified.
            if (info.Owner != null)
            {
                // Owner is the requester.
                if (info.Owner == requester.Id)
                    info.Owner = requester.Id;
                else
                {
                    // Find the relation between the owner and the requester.
                    var relationships = await _repositoryAccount.FindRelation(requester.Id, info.Owner.Value,
                        (byte) StatusAccount.Active);

                    // No relationship has been found.
                    if (relationships == null || relationships.Count < 1)
                    {
                        return Request.CreateResponse(HttpStatusCode.Forbidden, new
                        {
                            Error = $"{Language.WarnHasNoRelationship}"
                        });
                    }
                }
            }
            else
                info.Owner = requester.Id;

            // Retrieve the results list.
            var results = await _repositoryAllergy.FilterAllergy(info);

            // Filter allergies.
            var result = results.Allergies
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Cause,
                    x.Note,
                    x.Created,
                    x.LastModified
                });

            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Allergies = result,
                results.Total
            });
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
        private readonly IRepositoryAllergy _repositoryAllergy;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        ///     Service which is used for sending emails.
        /// </summary>
        private readonly IEmailService _emailService;

        #endregion
    }
}