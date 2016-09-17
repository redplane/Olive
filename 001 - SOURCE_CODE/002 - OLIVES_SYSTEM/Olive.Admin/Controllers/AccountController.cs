using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using log4net;
using OliveAdmin.Attributes;
using OliveAdmin.ViewModels.Edit;
using Shared.Enumerations;
using Shared.Interfaces;
using Shared.ViewModels.Filter;

namespace OliveAdmin.Controllers
{
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
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="editAccountViewModel"></param>
        /// <returns></returns>
        public async Task Edit(string email, EditAccountViewModel editAccountViewModel)
        {
            
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