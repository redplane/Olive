using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using log4net;
using OliveAdmin.Attributes;
using OliveAdmin.Interfaces;

namespace OliveAdmin.Controllers
{
    [MvcAuthorize]
    public class DoctorController : Controller
    {
        #region Constructors

        /// <summary>
        ///     Initialize an instance of DoctorController
        /// </summary>
        /// <param name="repositoryAccountExtended"></param>
        /// <param name="log"></param>
        public DoctorController(
            IRepositoryAccountExtended repositoryAccountExtended,
            ILog log)
        {
            _repositoryAccountExtended = repositoryAccountExtended;
            _log = log;
        }

        #endregion

        #region Methods

        /// <summary>
        /// This function is for rendering doctor management page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        
        /// <summary>
        ///     Filter doctors by using specific conditions.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Filter(FilterDoctorViewModel filter)
        {
            // Invalid data validation.
            if (!ModelState.IsValid)
                return View(filter);

            try
            {
                // Retrieve result from server.
                var filteredResult = await _repositoryAccountExtended.FilterDoctorsAsync(filter);

                return Json(filteredResult);
            }
            catch (Exception exception)
            {
                _log.Error(exception.Message, exception);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Instance of repository account.
        /// </summary>
        private readonly IRepositoryAccountExtended _repositoryAccountExtended;

        /// <summary>
        ///     Instance for logging management.
        /// </summary>
        private readonly ILog _log;

        #endregion
    }
}