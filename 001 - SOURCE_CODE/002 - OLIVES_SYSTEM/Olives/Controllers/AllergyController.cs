using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
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
using Shared.ViewModels;
using Shared.ViewModels.RequestCreate;

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
        public AllergyController(IRepositoryAccount repositoryAccount, IRepositoryAllergy repositoryAllergy, ILog log, IEmailService emailService)
        {
            _repositoryAccount = repositoryAccount;
            _repositoryAllergy = repositoryAllergy;
            _log = log;
            _emailService = emailService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Find a specialty by using specialty id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/allergy")]
        [HttpGet]
        [OlivesAuthorize(new[] { AccountRole.Doctor, AccountRole.Patient })]
        public async Task<HttpResponseMessage> Get([FromUri] int id)
        {
            #region Email & password of owners.

            var accountEmail = Request.Headers.Where(
                    x =>
                        !string.IsNullOrEmpty(x.Key) &&
                        x.Key.Equals(HeaderFields.RequestAccountEmail))
                    .Select(x => x.Value.FirstOrDefault())
                    .FirstOrDefault();

            var accountPassword = Request.Headers.Where(
                    x =>
                        !string.IsNullOrEmpty(x.Key) &&
                        x.Key.Equals(HeaderFields.RequestAccountPassword))
                    .Select(x => x.Value.FirstOrDefault()).FirstOrDefault();

            // Filter person by email & password.
            var person = _repositoryAccount.FindPerson(null, accountEmail, accountPassword, null);

            #endregion

            // Only filter and receive the first result.
            var filter = new AllergyGetViewModel();
            filter.Id = id;
            filter.Owner = person.Id;
            filter.Page = 0;
            filter.Records = 1;

            // Retrieve the results list.
            var results = await _repositoryAllergy.FilterAllergy(filter);

            // No result has been received.
            if (results == null || results.Allergies == null || results.Allergies.Count != 1)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Errors = new[] { Language.NoRecordHasBeenFound }
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
        /// Insert an allergy asyncrhonously.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/allergy")]
        [HttpPost]
        [OlivesAuthorize(new[] { AccountRole.Doctor, AccountRole.Patient })]
        public async Task<HttpResponseMessage> Post([FromBody] InitializeAllergyViewModel info)
        {
            #region ModelState result

            // Model hasn't been initialized.
            if (info == null)
            {
                _log.Error("Invalid allergies filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    Errors = new[] { Language.InvalidRequestParameters }
                });
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid allergies filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Email & password of owners.

            var accountEmail = Request.Headers.Where(
                    x =>
                        !string.IsNullOrEmpty(x.Key) &&
                        x.Key.Equals(HeaderFields.RequestAccountEmail))
                    .Select(x => x.Value.FirstOrDefault())
                    .FirstOrDefault();

            var accountPassword = Request.Headers.Where(
                    x =>
                        !string.IsNullOrEmpty(x.Key) &&
                        x.Key.Equals(HeaderFields.RequestAccountPassword))
                    .Select(x => x.Value.FirstOrDefault()).FirstOrDefault();

            // Filter person by email & password.
            var person = _repositoryAccount.FindPerson(null, accountEmail, accountPassword, null);
            if (person == null)
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, new
                {
                    Errors = new[] { Language.WarnNotAuthorizedAccount }
                });
            }

            #endregion

            // Only filter and receive the first result.
            var allergy = new Allergy();
            allergy.Owner = person.Id;
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
        /// Edit an allergy.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/allergy")]
        [HttpPut]
        [OlivesAuthorize(new[] { AccountRole.Doctor, AccountRole.Patient })]
        public async Task<HttpResponseMessage> Put([FromUri] int id, [FromBody] InitializeAllergyViewModel info)
        {
            #region ModelState result

            // Model hasn't been initialized.
            if (info == null)
            {
                _log.Error("Invalid allergies filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    Errors = new[] { Language.InvalidRequestParameters }
                });
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid allergies filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Header sections

            // Retrieve email from header.
            var accountEmail = Request.Headers.Where(
                    x =>
                        !string.IsNullOrEmpty(x.Key) &&
                        x.Key.Equals(HeaderFields.RequestAccountEmail))
                    .Select(x => x.Value.FirstOrDefault())
                    .FirstOrDefault();

            // Retrieve password from header.
            var accountPassword = Request.Headers.Where(
                    x =>
                        !string.IsNullOrEmpty(x.Key) &&
                        x.Key.Equals(HeaderFields.RequestAccountPassword))
                    .Select(x => x.Value.FirstOrDefault()).FirstOrDefault();

            #endregion

            // Find allergy by using allergy id and owner id.
            var allergies = await _repositoryAllergy.FindAllergyAsync(accountEmail, accountPassword, id);

            // Not record has been found.
            if (allergies == null || allergies.Count < 1)
            {
                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Errors = new[] { Language.WarnRecordNotFound }
                });
            }
            
            // Records are conflict.
            if (allergies.Count != 1)
            {
                // Tell front-end that records are conflict.
                return Request.CreateResponse(HttpStatusCode.Conflict, new
                {
                    Errors = new[] { Language.WarnRecordConflict }
                });
            }

            // Retrieve the first record.
            var allergy = allergies.FirstOrDefault();
            if (allergy == null)
            {
                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Errors = new[] { Language.WarnRecordNotFound }
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
        /// Delete an allergy.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/allergy")]
        [HttpDelete]
        [OlivesAuthorize(new[] { AccountRole.Doctor, AccountRole.Patient })]
        public async Task<HttpResponseMessage> Delete([FromUri] int id)
        {
            #region Header sections

            // Retrieve email from header.
            var accountEmail = Request.Headers.Where(
                    x =>
                        !string.IsNullOrEmpty(x.Key) &&
                        x.Key.Equals(HeaderFields.RequestAccountEmail))
                    .Select(x => x.Value.FirstOrDefault())
                    .FirstOrDefault();

            // Retrieve password from header.
            var accountPassword = Request.Headers.Where(
                    x =>
                        !string.IsNullOrEmpty(x.Key) &&
                        x.Key.Equals(HeaderFields.RequestAccountPassword))
                    .Select(x => x.Value.FirstOrDefault()).FirstOrDefault();

            #endregion

            // Find allergy by using allergy id and owner id.
            var allergies = await _repositoryAllergy.FindAllergyAsync(accountEmail, accountPassword, id);

            // Not record has been found.
            if (allergies == null || allergies.Count < 1)
            {
                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Errors = new[] { Language.WarnRecordNotFound }
                });
            }

            // Records are conflict.
            if (allergies.Count != 1)
            {
                // Tell front-end that records are conflict.
                return Request.CreateResponse(HttpStatusCode.Conflict, new
                {
                    Errors = new[] { Language.WarnRecordConflict }
                });
            }

            // Retrieve the first record.
            var allergy = allergies.FirstOrDefault();
            if (allergy == null)
            {
                // Tell front-end, no record has been found.
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Errors = new[] { Language.WarnRecordNotFound }
                });
            }

            // Remove the found allergy.
            _repositoryAllergy.DeleteAllergy(allergy);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
        
        /// <summary>
        /// Filter specialties by using specific conditions.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/allergy/filter")]
        [HttpPost]
        [OlivesAuthorize(new[] { AccountRole.Doctor, AccountRole.Patient })]
        public async Task<HttpResponseMessage> Filter([FromBody] AllergyGetViewModel info)
        {
            #region ModelState result

            // Model hasn't been initialized.
            if (info == null)
            {
                _log.Error("Invalid allergies filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, new
                {
                    Errors = new[] { Language.InvalidRequestParameters }
                });
            }

            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid allergies filter request parameters");
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));
            }

            #endregion

            #region Email & password of owners.

            var accountEmail = Request.Headers.Where(
                    x =>
                        !string.IsNullOrEmpty(x.Key) &&
                        x.Key.Equals(HeaderFields.RequestAccountEmail))
                    .Select(x => x.Value.FirstOrDefault())
                    .FirstOrDefault();

            var accountPassword = Request.Headers.Where(
                    x =>
                        !string.IsNullOrEmpty(x.Key) &&
                        x.Key.Equals(HeaderFields.RequestAccountPassword))
                    .Select(x => x.Value.FirstOrDefault()).FirstOrDefault();

            // Filter person by email & password.
            var person = _repositoryAccount.FindPerson(null, accountEmail, accountPassword, null);

            #endregion

            // Only see his/her own allergy.
            info.Owner = person.Id;

            // Retrieve the results list.
            var results = await _repositoryAllergy.FilterAllergy(info);

            // No result has been received.
            if (results == null || results.Allergies == null || results.Allergies.Count < 1)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, new
                {
                    Errors = new[] { Language.NoRecordHasBeenFound }
                });
            }

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