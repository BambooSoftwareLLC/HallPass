using System;
using System.Threading;
using System.Threading.Tasks;

namespace HallPass
{
    interface ITaskDelayer
    {
        Task DelayAsync(int milliseconds, CancellationToken cancellationToken = default);
        Task DelayAsync(TimeSpan timeSpan, CancellationToken cancellationToken = default);
    }
}
