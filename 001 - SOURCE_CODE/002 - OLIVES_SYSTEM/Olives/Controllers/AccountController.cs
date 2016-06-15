using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using log4net;
using Olives.ViewModels;
using Shared.Constants;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models;
using Shared.Models.Nodes;
using Shared.Resources;
using Shared.ViewModels;

namespace Olives.Controllers
{
    public class AccountController : ParentController
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of AccountController with Dependency injections.
        /// </summary>
        /// <param name="repositoryAccount"></param>
        public AccountController(IRepositoryAccount repositoryAccount, ILog log)
        {
            _repositoryAccount = repositoryAccount;
            _log = log;
        }

        #endregion

        #region Sign up

        /// <summary>
        ///     Register an account to Olive system.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        [Route("api/account/register")]
        [HttpPost]
        public async Task<HttpResponseMessage> Register([FromBody] OliveInitializePersonViewModel info)
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
            account.Role = AccountRole.Patient;
            account.Status = AccountStatus.Active;
            account.Role = info.Role;

            // TODO : Change status to Pending.
            account.Status = AccountStatus.Active;

            #region Initialize patient

            if (account.Role == AccountRole.Patient)
            {
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

            #region Initialize doctor

            // Initialize person into database.
            var result = await _repositoryAccount.InitializePerson(account);

            // Cannot create an account to system.
            if (result == null)
            {
                // Initialize error response to client.
                responseError.Errors.Add(Language.InternalServerError);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, responseError);
            }

            // Tell doctor to wait for admin confirmation.
            return Request.CreateResponse(HttpStatusCode.OK, new
            {
                User = result.Data,
                Messages = new[] {Language.WaitForAdminConfirmation}
            });

            #endregion
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

        #endregion

        #region Login

        [Route("api/account/index")]
        [HttpGet]
        public HttpResponseMessage Index()
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var viewPath = HttpContext.Current.Server.MapPath(@"~/Views/Account/Index.html");
            var info = File.ReadAllText(viewPath);
            response.Content = new StringContent(info);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        /// <summary>
        ///     This function is for authenticate an user account.
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <returns></returns>
        [Route("api/account/login")]
        [HttpPost]
        public async Task<HttpResponseMessage> Login([FromBody] LoginViewModel loginViewModel)
        {
            // Invalid model state.
            if (!ModelState.IsValid)
            {
                _log.Error("Invalid login request parameters");
                return Request.CreateResponse(HttpStatusCode.BadGateway, RetrieveValidationErrors(ModelState));
            }

            // Pass parameter to login function. 
            var result = await _repositoryAccount.LoginAsync(loginViewModel);

            // If no result return, that means no account.
            if (result == null)
            {
                _log.Error(
                    $"No record has been found with {loginViewModel.Email} - {loginViewModel.Password} - {loginViewModel.Role} ");
                ModelState.AddModelError("Credential", Language.InvalidLoginInfo);
                return Request.CreateResponse(HttpStatusCode.NotFound, RetrieveValidationErrors(ModelState));
            }
            // Requested user is not a patient or a doctor.
            if (result.Role != AccountRole.Patient && result.Role != AccountRole.Doctor)
            {
                _log.Error($"{loginViewModel.Email} is a admin, therefore, it cannot be used here.");
                ModelState.AddModelError("Credential", Language.InvalidLoginInfo);
                return Request.CreateResponse(HttpStatusCode.NotFound, RetrieveValidationErrors(ModelState));
            }

            if (result.Status == AccountStatus.Pending)
            {
                // Tell doctor to contact admin for account verification.
                if (result.Role == AccountRole.Doctor)
                {
                    _log.Error($"Access is forbidden because {loginViewModel.Email} is waiting for admin confirmation");
                    return Request.CreateResponse(HttpStatusCode.Forbidden, new
                    {
                        Errors = new[] {Language.DoctorAccountPending}
                    });
                }

                _log.Error($"Access is forbidden because {loginViewModel.Email} is waiting for admin confirmation");
                // Tell patient to access his/her email to verify the account.
                return Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Errors = new[] {Language.PatientAccountPending}
                });
            }

            _log.Info($"{loginViewModel.Email} has logged in successfully");
            return Request.CreateResponse(HttpStatusCode.OK, new {User = result});
        }

        #endregion
    }
}