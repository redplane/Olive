using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using log4net;
using Olives.Interfaces;
using Olives.ViewModels.Filter;
using Shared.Enumerations;
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
        private readonly IRepositoryAccountExtended _repositoryAccountExtended;

        private readonly IRepositoryToken _repositoryToken;

        #region Constructors

        /// <summary>
        ///     Initialize an instance of AccountVerifyController with given information.
        /// </summary>
        public ServiceController(
            IRepositoryAccountExtended repositoryAccountExtended, IRepositoryToken repositoryToken,
            ILog log)
        {
            _repositoryAccountExtended = repositoryAccountExtended;
            _repositoryToken = repositoryToken;
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

                return View(ViewBag);
            }

            try
            {
                #region Find the account token

                // Find the activation token.
                var filter = new FilterAccountTokenViewModel();
                filter.Code = code;
                filter.Type = (byte)TypeAccountCode.Activation;

                var accountToken = await _repositoryToken.FindAccountTokenAsync(filter);

                // Token is not found.
                if (accountToken == null)
                {
                    _log.Error($"Token [Code: {code}] is not found");
                    ViewBag.Message = Language.MessageInvalidActivationCode;
                    ViewBag.IsError = true;

                    return View(ViewBag);
                }

                #region Token validate

                // Token has been expired.
                if (DateTime.UtcNow > accountToken.Expired)
                {
                    _log.Error($"Token [Code: {code}] was expired at {accountToken.Expired.ToString("G")}");
                    ViewBag.Message = Language.MessageTokenIsExpired;
                    ViewBag.IsError = true;

                    return View(ViewBag);
                }

                #endregion

                #region Account find

                // Find the owner of code.
                var account =
                    await
                        _repositoryAccountExtended.FindPersonAsync(accountToken.Owner, null, null, (byte)Role.Patient, StatusAccount.Pending);

                // Account is not found.
                if (account == null)
                {
                    _log.Error($"Account [Id: {filter.Owner}] is not found.");
                    ViewBag.Message = Language.MessageEmailNotFoundAsPending;
                    ViewBag.IsError = true;

                    return View(ViewBag);
                }

                #endregion

                #region Account update

                // Active the account.
                account.Status = (byte) StatusAccount.Active;

                // Save the account.
                await _repositoryAccountExtended.InitializePersonAsync(account);

                try
                {
                    await _repositoryToken.DetachAccountToken(filter);
                }
                catch (Exception exception)
                {
                    _log.Error(exception.Message, exception);   
                }

                #endregion
                
                #endregion

            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);
                ViewBag.Message = Language.WarnInternalServerError;
                ViewBag.IsError = true;
            }

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