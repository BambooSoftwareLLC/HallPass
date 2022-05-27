using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace HallPass
{
    internal class TokenBucket : IBucket
    {
        private readonly ConcurrentQueue<Ticket> _tickets = new ConcurrentQueue<Ticket>();
        private readonly ConcurrentQueue<Guid> _refillQueue = new ConcurrentQueue<Guid>();
        
        private readonly ITimeService _timeService;
        private readonly ITaskDelayer _taskDelayer;

        private readonly int _periodDurationMilliseconds;
        private readonly int _requestsPerPeriod;
        private DateTimeOffset _lastRefill = DateTimeOffset.MinValue;

        public TokenBucket(int requestsPerPeriod, int periodDuration, TimeUnit timeUnit, ITimeService timeService, ITaskDelayer taskDelayer)
        {
            _requestsPerPeriod = requestsPerPeriod;
            _timeService = timeService;
            _taskDelayer = taskDelayer;

            var periodDurationMilliseconds = periodDuration * 1000;
            if (timeUnit.Equals(TimeUnit.Minutes)) periodDurationMilliseconds *= 60;
            if (timeUnit.Equals(TimeUnit.Hours)) periodDurationMilliseconds *= 60 * 60;
            _periodDurationMilliseconds = periodDurationMilliseconds;
        }

        public async Task<Ticket> GetTicketAsync(CancellationToken cancellationToken = default)
        {
            Ticket ticket;
            while (!_tickets.TryDequeue(out ticket))
            {
                await RefillAsync(cancellationToken);
            }

            return ticket;
        }

        private async Task RefillAsync(CancellationToken cancellationToken = default)
        {
            // add myself to the refill queue
            var token = Guid.NewGuid();
            _refillQueue.Enqueue(token);

            // peek the refill queue in a loop, waiting for the calculated wait time between each loop
            var waitTime = GetWaitTime();
            if (waitTime > TimeSpan.Zero)
                await _taskDelayer.DelayAsync(waitTime, cancellationToken);

            while (_refillQueue.TryPeek(out var nextToken) && !nextToken.Equals(token))
            {
                waitTime = GetWaitTime();
                if (waitTime > TimeSpan.Zero)
                    await _taskDelayer.DelayAsync(waitTime, cancellationToken);
            }

            // if myself is next, wait for the remaining wait time
            waitTime = GetWaitTime();
            if (waitTime > TimeSpan.Zero)
                await _taskDelayer.DelayAsync(waitTime, cancellationToken);

            // refill the tickets
            for (int i = 0; i < _requestsPerPeriod; i++)
            {
                _tickets.Enqueue(Ticket.Blank);
            }

            // modify timestamp for last refill
            _lastRefill = _timeService.GetNow();

            // dequeue self from refill queue
            _refillQueue.TryDequeue(out var _);
        }

        private TimeSpan GetWaitTime()
        {
            // if negative, return 0
            var waitTime = _lastRefill.AddMilliseconds(_periodDurationMilliseconds) - _timeService.GetNow();
            return waitTime.TotalSeconds < 0
                ? TimeSpan.Zero
                : waitTime;
        }
    }
}
