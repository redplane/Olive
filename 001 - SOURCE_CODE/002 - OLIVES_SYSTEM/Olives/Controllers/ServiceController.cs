using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using log4net;
using Shared.Interfaces;
using Shared.Resources;

namespace Olives.Controllers
{
    public class ServiceController : Controller
    {
        /// <summary>
        ///     Instance of module which is used for logging.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        ///     Repository which provides functions to access account database.
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

        /// <summary>
        ///     Repository which provides functions to access activation code database.
        /// </summary>
        private readonly IRepositoryActivationCode _repositoryActivationCode;

        #region Constructors

        /// <summary>
        ///     Initialize an instance of AccountVerifyController with given information.
        /// </summary>
        public ServiceController(IRepositoryAccount repositoryAccount,
            IRepositoryActivationCode repositoryActivationCode, ILog log)
        {
            _repositoryAccount = repositoryAccount;
            _repositoryActivationCode = repositoryActivationCode;
            _log = log;
        }

        #endregion

        /// <summary>
        ///     This function checks the activation code, validate it and returns result to client.
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> Verify(string code)
        {
            // Invalid activation code.
            if (string.IsNullOrWhiteSpace(code))
            {
                _log.Error("Activation code is null or empty. It's invalid");
                ViewBag.Message = Language.MessageInvalidActivationCode;
                ViewBag.IsError = true;
            }

            try
            {
                // Activate the account.
                await _repositoryAccount.InitializePatientActivation(code);
            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);
                ViewBag.Message = Language.WarnInternalServerError;
                ViewBag.IsError = true;
            }

            ViewBag.Message = Language.MessageAccountActivatedSuccessfully;
            ViewBag.IsError = false;

            return View();
        }

        /// <summary>
        ///     This function is for displaying a view to find password.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult FindPassword()
        {
            return View();
        }
    }
}