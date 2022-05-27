using System;

namespace HallPass
{
    internal interface ITimeService
    {
        DateTimeOffset GetNow();
    }
}