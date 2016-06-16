using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using log4net;
using Olives.Interfaces;
using Olives.ViewModels;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Models.Nodes;
using Shared.Resources;
using Shared.ViewModels;

namespace Olives.Controllers
{
    public class PatientController : ParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of AccountController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        /// <param name="log"></param>
        /// <param name="emailService"></param>
        public PatientController(IRepositoryAccount repositoryAccount, ILog log, IEmailService emailService)
        {
            _repositoryAccount = repositoryAccount;
            _log = log;
            _emailService = emailService;
        }

        #endregion

        #region Sign up

        /// <summary>
        ///     Register an account to Olive system.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/patient/register")]
        [HttpPost]
        public async Task<HttpResponseMessage> Register([FromBody] InitializePersonViewModel info)
        {
            // Error response initialization.
            var responseError = new ResponseErrror();
            responseError.Errors = new List<string>();

            #region ModelState validation

            // Information hasn't been initialize.
            if (info == null)
            {
                // Initialize the default instance and do the validation.
                info = new OliveInitializePersonViewModel();
                Validate(info);
            }

            // Invalid model state.
            if (!ModelState.IsValid)
                return Request.CreateResponse(HttpStatusCode.BadRequest, RetrieveValidationErrors(ModelState));

            #endregion

            #region Database record validation

            // Check whether email has been used or not.
            var person = _repositoryAccount.FindPerson(info.Email, null, null);
            if (person != null)
            {
                responseError.Errors.Add(Language.EmailExistInSystem);
                return Request.CreateResponse(HttpStatusCode.Conflict, responseError);
            }

            #endregion

            // Account initialization.
            var account = new Person();
            account.Id = Guid.NewGuid().ToString("N");
            account.FirstName = info.FirstName;
            account.LastName = info.LastName;
            account.Birthday = info.Birthday;
            account.Gender = info.Gender;
            account.Email = info.Email;
            account.Password = info.Password;
            account.Phone = info.Phone;
            account.Money = 0;
            account.Created = DateTime.Now.Ticks;
            account.Status = AccountStatus.Active;
            account.Role = AccountRole.Patient;

            // TODO : Change status to Pending.
            account.Status = AccountStatus.Active;

            var activationCode = new ActivationCode();
            activationCode.Code = Guid.NewGuid().ToString();
            activationCode.Expire = DateTime.Now.AddHours(24).Ticks;

            var patientResult = await _repositoryAccount.InitializePerson(account, activationCode);
            if (patientResult == null)
            {
                // Initialize error response to client.
                responseError.Errors.Add(Language.InternalServerError);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, responseError);
            }

            // TODO: Send mail with activation code to client.

            // Tell client to check his/her email to verify this account.
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                User = patientResult.Person,
                Messages = new[] {Language.AccessEmailForVerification}
            });
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository of accounts
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

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