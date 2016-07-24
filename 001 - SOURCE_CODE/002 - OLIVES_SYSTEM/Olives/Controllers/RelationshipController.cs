using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Attributes;
using Olives.Models;
using Olives.ViewModels.Filter;
using Olives.ViewModels.Initialize;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Helpers;
using Shared.Interfaces;
using Shared.Models;
using Shared.Resources;

namespace Olives.Controllers
{
    public class RelationshipController : ApiParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of AccountController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        /// <param name="repositoryRelation"></param>
        /// <param name="applicationSetting"></param>
        /// <param name="log"></param>
        public RelationshipController(IRepositoryAccount repositoryAccount, IRepositoryRelation repositoryRelation,
            ApplicationSetting applicationSetting, ILog log)
        {
            _repositoryAccount = repositoryAccount;
            _repositoryRelation = repositoryRelation;
            _applicationSetting = applicationSetting;
            _log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Request to create a relationship to a target person.
        /// </summary>
        /// <param name="initializer"></param>
        /// <returns></returns>
        [Route("api/relationship")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Patient})]
        public async Task<HttpResponseMessage> InitializeRelation([FromBody] InitializeRelationshipViewModel initializer)
        {
            #region Request parameters validation

            // Initializer hasn't been initializer
            if (initializer == null)
            {
                initializer = new InitializeRelationshipViewModel();
                Validate(initializer);
            }

            // Request parameters are invalid.
            if (!ModelState.IsValid)
            {
                // Log the error.
                _log.Error("Request parameters are invalid. Errors sent to client");

                // Tell the client about this error.
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            try
            {
                // Retrieve information of person who sent request.
                var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

                // Find the target.
                var person =
                    await _repositoryAccount.FindPersonAsync(initializer.Target, null, null, null, StatusAccount.Active);

                // Cannot find the target.
                if (person == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnTargetAccountNotFound}"
                    });
                }

                // Check whether these two people have relation or not.
                var relationship =
                    await _repositoryRelation.FindRelationshipAsync(requester.Id, initializer.Target, null);

                // 2 people already make a relationship to each other.
                if (relationship != null && relationship.Count > 0)
                {
                    // Relationship has already been registered.
                    _log.Error($"Relationship from Requester[Id: {requester.Id}] to Owner[Id: {person.Id}] exists.");

                    // Tell client about the conflict.
                    return Request.CreateResponse(HttpStatusCode.Conflict, new
                    {
                        Error = $"{Language.WarnRelationshipAlreadyExist}"
                    });
                }

                // Create an instance of relation.
                var relation = new Relation();
                relation.Source = requester.Id;
                relation.SourceFirstName = requester.FirstName;
                relation.SourceLastName = requester.LastName;
                relation.Target = initializer.Target;
                relation.TargetFirstName = person.FirstName;
                relation.TargetLastName = person.LastName;
                relation.Created = EpochTimeHelper.Instance.DateTimeToEpochTime(DateTime.UtcNow);
                relation.Status = (byte) StatusRelation.Pending;

                await _repositoryRelation.InitializeRelationAsync(relation);

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Relationship = new
                    {
                        relation.Id,
                        Source = new
                        {
                            Id = relation.Source,
                            FirstName = relation.SourceFirstName,
                            LastName = relation.SourceLastName
                        },
                        Target = new
                        {
                            Id = relation.Target,
                            FirstName = relation.TargetFirstName,
                            LastName = relation.TargetLastName
                        },
                        relation.Created,
                        relation.Status
                    }
                });
            }
            catch (Exception exception)
            {
                // Exception happens, log the error and tell client about the error.
                _log.Error(exception.Message, exception);

                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        ///     Confirm a pending relation.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/relationship/confirm")]
        [HttpPost]
        [OlivesAuthorize(new[] {Role.Patient, Role.Doctor})]
        public async Task<HttpResponseMessage> ConfirmRemoveRelationship([FromBody] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            // Find the relationship by using id.
            var relationship =
                await
                    _repositoryRelation.FindRelationshipAsync(id, requester.Id, RoleRelationship.Target,
                        (byte) StatusRelation.Pending);

            // No relationship has been returned.
            if (relationship == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Error = $"{Language.WarnRelationNotFound}"
                });
            }

            relationship.Status = (byte) StatusRelation.Active;
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Relationship = new
                {
                    relationship.Id,
                    Source = new
                    {
                        Id = relationship.Source,
                        FirstName = relationship.SourceFirstName,
                        LastName = relationship.SourceLastName
                    },
                    Target = new
                    {
                        Id = relationship.Target,
                        FirstName = relationship.TargetFirstName,
                        LastName = relationship.TargetLastName
                    },
                    relationship.Created,
                    relationship.Status
                }
            });
        }

        /// <summary>
        ///     Remove an active relation.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/relationship")]
        [HttpDelete]
        [OlivesAuthorize(new[] {Role.Patient, Role.Doctor})]
        public async Task<HttpResponseMessage> RemoveRelationship([FromUri] int id)
        {
            // Retrieve information of person who sent request.
            var requester = (Person) ActionContext.ActionArguments[HeaderFields.RequestAccountStorage];

            try
            {
                // Delete relationship and retrieve the number of affected records.
                var records = await _repositoryRelation.DeleteRelationAsync(id, requester.Id, null, null);
                if (records < 1)
                {
                    // No record has been found. Log the error for future trace.
                    _log.Error($"There is no relationship [Id: {id}].");

                    // Tell the client about the rror.
                    return Request.CreateResponse(HttpStatusCode.NotFound, new
                    {
                        Error = $"{Language.WarnRecordNotFound}"
                    });
                }
                return Request.CreateResponse(HttpStatusCode.OK);
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
                    _repositoryRelation.FilterRelatedDoctorAsync(requester.Id, filter.Status, filter.Page,
                        filter.Records);

            // Throw the list back to client.
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                Relationships = result.List.Select(x => new
                {
                    Doctor = new
                    {
                        x.Doctor.Id,
                        x.Doctor.Person.FirstName,
                        x.Doctor.Person.LastName,
                        Specialty = new
                        {
                            Id = x.Doctor.SpecialtyId,
                            Name = x.Doctor.SpecialtyName
                        },
                        x.Doctor.Rank,
                        x.Doctor.Person.Address,
                        Photo =
                            InitializeUrl(_applicationSetting.AvatarStorage.Relative, x.Doctor.Person.Photo,
                                Values.StandardImageExtension)
                    },
                    Status = x.RelationshipStatus,
                    x.Created
                }),
                result.Total
            });
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository of accounts
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

        /// <summary>
        ///     Repository of relationships.
        /// </summary>
        private readonly IRepositoryRelation _repositoryRelation;

        /// <summary>
        ///     Instance stores application settings.
        /// </summary>
        private readonly ApplicationSetting _applicationSetting;

        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}