using System;

namespace Shared.Interfaces
{
    public interface ITimeService
    {
        double DateTimeUtcToUnix(DateTime dateTime);
        
        DateTime UnixToDateTimeUtc(double unixTime);
    }
}