using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Constants;
using Olives.Interfaces;
using Olives.ViewModels.Filter;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;
using Shared.ViewModels.Filter;
using FilterRelationshipViewModel = Olives.ViewModels.Filter.FilterRelationshipViewModel;

namespace Olives.Controllers
{
    [Route("api/relationship")]
    public class RelationshipController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of AccountController with Dependency injections.
        /// </summary>
        /// <param name="repositoryRelation"></param>
        /// <param name="repositoryStorage"></param>
        /// <param name="log"></param>
        public RelationshipController(
            IRepositoryRelationship repositoryRelation,
            IRepositoryStorage repositoryStorage,
            ILog log)
        {
            _repositoryRelation = repositoryRelation;
            _repositoryStorage = repositoryStorage;
            _log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Remove an active relation.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [OlivesAuthorize(new[] {Role.Patient, Role.Doctor})]
        public async Task<HttpResponseMessage> DeleteRelationship([FromUri] int id)
        {
            try
            {
                // Retrieve information of person who sent request.
                var requester = (Person)ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];
                
                // Filter initialization.
                var filter = new FilterRelationshipViewModel();
                filter.Requester = requester;
                filter.Id = id;

                // Count the number of deleted records.
                var records = await _repositoryRelation.DeleteRelationAsync(filter);

                // Delete successfully
                if (records >= 1)
                    return Request.CreateResponse(HttpStatusCode.OK);

                // No record has been found. Log the error for future trace.
                _log.Error($"There is no relationship [Id: {id}].");

                // Tell the client about the rror.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRecordNotFound}"
                });
            }
            catch (Exception exception)
            {
                // Log the exception to file.
                _log.Error(exception.Message, exception);

                // Tell client there is a problem about this server, please try again.
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Error = $"{Language.WarnInternalServerError}"
                });
            }
        }

        /// <summary>
        ///     Filter relationship by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/relationship/filter/doctor")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> FilterRelatedDoctor([FromBody] FilterRelatedPeopleViewModel filter)
        {
            #region Parameters validation

            // Filter hasn't been initialized.
            if (filter == null)
            {
                filter = new FilterRelatedPeopleViewModel();
                Validate(filter);
            }

            // Validation is not successful.
            if (!ModelState.IsValid)
            {
                _log.Error("Parameters are invalid. Errors sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Filter the relationship.
            var result =
                await
                    _repositoryRelation.FilterRelatedDoctorAsync(requester.Id, filter.Page,
                        filter.Records);

            // Find the avatar storage.
            var storageAvatar = _repositoryStorage.FindStorage(Storage.Avatar);

            // Throw the list back to client.
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Relationships = result.List.Select(x => new
                {
                    x.Relation,
                    Doctor = new
                    {
                        x.Doctor.Id,
                        x.Doctor.Person.FirstName,
                        x.Doctor.Person.LastName,
                        Specialty = new
                        {
                            x.Doctor.Specialty.Id,
                            x.Doctor.Specialty.Name
                        },
                        x.Doctor.Rank,
                        x.Doctor.Person.Address,
                        Photo =
                            InitializeUrl(storageAvatar.Relative, x.Doctor.Person.Photo,
                                Values.StandardImageExtension),
                        x.Doctor.Person.Phone,
                        x.Doctor.Person.Email
                    },
                    Status = x.RelationshipStatus,
                    x.Created
                }),
                result.Total
            });
        }

        /// <summary>
        ///     Filter relationship by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [Route("api/relationship/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Patient, Role.Doctor})]
        public async Task<HttpResponseMessage> FilterRelationship([FromBody] FilterRelationshipViewModel filter)
        {
            #region Parameters validation

            // Filter hasn't been initialized.
            if (filter == null)
            {
                filter = new FilterRelationshipViewModel();
                Validate(filter);
            }

            // Validation is not successful.
            if (!ModelState.IsValid)
            {
                _log.Error("Parameters are invalid. Errors sent to client.");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Result handling

            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Update the filter.
            filter.Requester = requester;

            // Filter the relationship.
            var result =
                await
                    _repositoryRelation.FilterRelationshipAsync(filter);

            // Find the storage of avatar.
            var storageAvatar = _repositoryStorage.FindStorage(Storage.Avatar);

            // Throw the list back to client.
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Relationships = result.Relationships.Select(x => new
                {
                    x.Id,
                    Source = new
                    {
                        Id = x.Source,
                        x.Patient.Person.FirstName,
                        x.Patient.Person.LastName,
                        Photo =
                            InitializeUrl(storageAvatar.Relative, x.Patient.Person.Photo, Values.StandardImageExtension)
                    },
                    Target = new
                    {
                        Id = x.Target,
                        x.Doctor.Person.FirstName,
                        x.Doctor.Person.LastName,
                        Photo =
                            InitializeUrl(storageAvatar.Relative, x.Doctor.Person.Photo, Values.StandardImageExtension)
                    },
                    x.Created
                }),
                result.Total
            });

            #endregion
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository of relationships.
        /// </summary>
        private readonly IRepositoryRelationship _repositoryRelation;

        /// <summary>
        ///     Repository of storage.
        /// </summary>
        private readonly IRepositoryStorage _repositoryStorage;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}