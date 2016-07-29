using System;

namespace Olives.Interfaces
{
    public interface ITimeService
    {
        double DateTimeUtcToUnix(DateTime dateTime);
        
        DateTime UnixToDateTimeUtc(double unixTime);
    }
}