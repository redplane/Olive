using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using log4net;
using OliveAdmin.Attributes;
using OliveAdmin.Models;
using OliveAdmin.Resources;
using OliveAdmin.ViewModels.Edit;
using OliveAdmin.ViewModels.Responses.Errors;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.Models.Vertexes;
using Shared.ViewModels.Filter;

namespace OliveAdmin.Controllers
{
    [RoutePrefix("Account")]
    [MvcAuthorize(new[] { Role.Admin })]
    public class AccountController : MvcController
    {
        /// <summary>
        /// Provides functions to access account database.
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

        /// <summary>
        /// Provides functions to access logging.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// Initialize controller with dependency injections.
        /// </summary>
        public AccountController(IRepositoryAccount repositoryAccount, ILog log)
        {
            _repositoryAccount = repositoryAccount;
            _log = log;
        }

        [HttpGet]
        [Route("")]
        [Route("Index")]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// This function is for changing account information.
        /// </summary>
        /// <param name="editAccountViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Info")]
        public async Task<ActionResult> ChangeAccountInformation(EditAccountViewModel editAccountViewModel)
        {
            // Find account information from session.
            var requester = (Account) Session[Constant.MvcAccount];

            // Request parameters are invalid.
            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int) HttpStatusCode.BadRequest;
                Response.TrySkipIisCustomErrors = true;

                return Json(new HttpBadRequestViewModel()
                {
                    Errors = FindValidationError(ModelState)
                });
            }
            
            // Targeted email is the  requester email.
            if (editAccountViewModel.Email.Equals(requester.Email, StringComparison.InvariantCultureIgnoreCase))
            {
                // Tell the client, this request is forbidden.
                Response.StatusCode = (int) HttpStatusCode.Forbidden;
                return Json(new HttpErrorViewModel()
                {
                    Message = MvcErrorCode.CantChangeSelfStatus
                });
            }

            // Account filter initialization.
            var filterAccountViewModel = new FilterAccountViewModel();
            filterAccountViewModel.Email = editAccountViewModel.Email;
            filterAccountViewModel.EmailComparision = TextComparision.Equal;

            // Find the account using filter.
            var account = await _repositoryAccount.FindAccountAsync(filterAccountViewModel);

            // Account is not found.
            if (account == null)
            {
                _log.Error($"Account {editAccountViewModel.Email} is not found in database");

                Response.StatusCode = (int) HttpStatusCode.NotFound;
                return Json(new HttpErrorViewModel()
                {
                    Message = MvcErrorCode.AccountIsNotFound
                });
            }

            #region Account information update

            // This flag is for checking whether account information has been changed or not.
            var isAccountUpdated = false;

            // First name needs updating.
            if (!string.IsNullOrWhiteSpace(editAccountViewModel.FirstName))
            {
                account.FirstName = editAccountViewModel.FirstName;
                isAccountUpdated = true;
            }

            // Last name needs updating.
            if (!string.IsNullOrWhiteSpace(editAccountViewModel.LastName))
            {
                account.LastName = editAccountViewModel.LastName;
                isAccountUpdated = true;
            }

            // Password needs updating.
            if (!string.IsNullOrWhiteSpace(editAccountViewModel.Password))
            {
                account.Password = _repositoryAccount.FindEncryptedPassword(editAccountViewModel.Password);
                isAccountUpdated = true;
            }

            // Status needs updating.
            if (editAccountViewModel.Status != null)
            {
                account.Status = editAccountViewModel.Status.Value;
                isAccountUpdated = true;
            }

            // Gender needs updating.
            if (editAccountViewModel.Gender != null)
            {
                account.Gender = editAccountViewModel.Gender.Value;
                isAccountUpdated = true;
            }

            // Birthday needs updating.
            if (editAccountViewModel.Birthday != null)
            {
                account.Birthday = editAccountViewModel.Birthday.Value;
                isAccountUpdated = true;
            }

            #endregion

            // Only update account to database as at least one information has been updated.
            //if (isAccountUpdated) account = await _repositoryAccount.InitializeAccountAsync(account);
            
            // Tell the client the update is successful.
            return Json(account);
        }
        
        /// <summary>
        /// This function is for filtering accounts and return response to front-end.
        /// </summary>
        /// <param name="filterAccountViewModel"></param>
        /// <returns></returns>
        public async Task<ActionResult> Filter(FilterAccountViewModel filterAccountViewModel)
        {
            try
            {
                // Request parameters are invalid.
                if (!ModelState.IsValid)
                {
                    // Custom iis error page shouldn't be shown.
                    Response.TrySkipIisCustomErrors = true;

                    // Treat the response as the response of bad request.
                    Response.StatusCode = (int) HttpStatusCode.BadRequest;
                    
                    // Retrieve list of error messages.
                    return Json(new
                    {
                        Errors = FindValidationError(ModelState)
                    });
                }

                var filteredResult = await _repositoryAccount.FilterAccountsAsync(filterAccountViewModel);
                return Json(filteredResult);
            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

    }
}