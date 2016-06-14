﻿using System.Web.Http;
using Shared.Interfaces;

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