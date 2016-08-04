using System;
using Shared.Interfaces;

namespace Shared.Services
{
    public class TimeService : ITimeService
    {
        /// <summary>
        /// Calculate the unix time from UTC DateTime.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public double DateTimeUtcToUnix(DateTime dateTime)
        {
            return (dateTime - _utcDateTime).TotalMilliseconds;
        }

        /// <summary>
        /// Convert unix time to UTC time.
        /// </summary>
        /// <param name="unixTime"></param>
        /// <returns></returns>
        public DateTime UnixToDateTimeUtc(double unixTime)
        {
            return _utcDateTime.AddMilliseconds(unixTime);
        }

        #region Properties

        private readonly DateTime _utcDateTime;

        public TimeService()
        {
            _utcDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        #endregion
    }
}