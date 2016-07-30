using System;
using Shared.Interfaces;

namespace Shared.Services
{
    public class TimeService : ITimeService
    {
        public double DateTimeUtcToUnix(DateTime dateTime)
        {
            return (dateTime - _utcDateTime).TotalMilliseconds;
        }

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