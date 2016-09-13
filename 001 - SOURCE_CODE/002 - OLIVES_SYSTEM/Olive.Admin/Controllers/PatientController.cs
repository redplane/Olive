using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using log4net;
using OliveAdmin.Attributes;
using OliveAdmin.Interfaces;
using OliveAdmin.ViewModels.Filter;

namespace OliveAdmin.Controllers
{
    [MvcAuthorize]
    public class PatientController : Controller
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of AdminController.
        /// </summary>
        /// <param name="repositoryAccountExtended"></param>
        /// <param name="log"></param>
        public PatientController(
            IRepositoryAccountExtended repositoryAccountExtended,
            ILog log)
        {
            _repositoryAccountExtended = repositoryAccountExtended;
            _log = log;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Repository account DI
        /// </summary>
        private readonly IRepositoryAccountExtended _repositoryAccountExtended;

        /// <summary>
        ///     Instance which provides functions for logging.
        /// </summary>
        private readonly ILog _log;

        #endregion

        #region Methods

        /// <summary>
        ///     Filter patient by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Filter(FilterPatientViewModel filter)
        {
            try
            {
                // Invalid model state.
                if (!ModelState.IsValid)
                    return View(filter);

                // Filter patient by using specific conditions.
                var filteredResult = await _repositoryAccountExtended.FilterPatientsAsync(filter);

                return Json(filteredResult);
            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
            
        }

        #endregion
    }
}