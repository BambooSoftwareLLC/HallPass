using System;

namespace HallPass
{
    class TimeService : ITimeService
    {
        public DateTimeOffset GetNow() => DateTimeOffset.Now;
    }
}
