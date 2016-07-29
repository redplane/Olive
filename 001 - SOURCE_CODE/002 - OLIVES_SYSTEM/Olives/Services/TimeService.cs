using System;
using Olives.Interfaces;

namespace Olives.Services
{
    public class TimeService : ITimeService
    {
        #region Properties

        private readonly DateTime _utcDateTime;

        public TimeService()
        {
            _utcDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        #endregion
        
        public double DateTimeUtcToUnix(DateTime dateTime)
        {
            return (dateTime - _utcDateTime).TotalMilliseconds;
        }
        
        public DateTime UnixToDateTimeUtc(double unixTime)
        {
            return _utcDateTime.AddMilliseconds(unixTime);
        }
    }
}