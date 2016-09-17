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
        
        #endregion
    }
}