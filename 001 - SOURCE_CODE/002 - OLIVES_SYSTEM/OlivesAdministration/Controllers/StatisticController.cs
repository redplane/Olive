using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using OlivesAdministration.Attributes;
using OlivesAdministration.ViewModels;
using Shared.Constants;
using Shared.Interfaces;
using Shared.Models.Nodes;
using Shared.Resources;
using Shared.ViewModels;

namespace OlivesAdministration.Controllers
{
    [Route("api/statistic")]
    public class StatisticController : ParentController
    {
        #region Properties

        /// <summary>
        ///     Instance of repository account.
        /// </summary>
        private readonly IRepositoryAccount _repositoryAccount;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initialize an instance of DoctorController
        /// </summary>
        /// <param name="repositoryAccount"></param>
        public StatisticController(IRepositoryAccount repositoryAccount)
        {
            _repositoryAccount = repositoryAccount;
        }

        #endregion
        
        [Route("api/statistic/doctor/status")]
        public void SummaryDoctorStatus()
        {
            
        }
    }
}