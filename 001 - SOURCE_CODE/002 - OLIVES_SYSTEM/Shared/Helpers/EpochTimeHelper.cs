using System;
namespace Shared.Helpers
{
    public class EpochTimeHelper
    {
        #region Properties

        /// <summary>
        /// Time milestone which is used for EpochCalculation.
        /// </summary>
        private readonly DateTime _timeMilestone;

        /// <summary>
        /// Static instance of EpochTimeHelper.
        /// </summary>
        private static EpochTimeHelper _instance;

        /// <summary>
        /// - As available, EpochTimeHelper instance will be returned.
        /// - Otherwise, initialize it before return.
        /// </summary>
        public static EpochTimeHelper Instance
        {
            get
            {
                return _instance ?? (_instance = new EpochTimeHelper());
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initialize an instance of EpochTimeHelper with settings.
        /// </summary>
        public EpochTimeHelper()
        {
            // By default, milestone is Jan 1st 1970.
            _timeMilestone = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        #endregion

        /// <summary>
        /// Calculate epoch datetime from DateTime instance.
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public double DateTimeToEpochTime(DateTime time)
        {
            // Convert datetime to UTC, subtract with the base and count the total millisecs.
            return time.ToUniversalTime().Subtract(_timeMilestone).TotalMilliseconds;
        }
    }
}