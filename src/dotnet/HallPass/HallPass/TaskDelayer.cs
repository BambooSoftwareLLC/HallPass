﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace HallPass
{
    class TaskDelayer : ITaskDelayer
    {
        public async Task DelayAsync(int milliseconds, CancellationToken cancellationToken = default) => await Task.Delay(milliseconds, cancellationToken);
        public async Task DelayAsync(TimeSpan timeSpan, CancellationToken cancellationToken = default) => await Task.Delay(timeSpan, cancellationToken);
    }
}