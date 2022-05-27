using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HallPass.UnitTests
{
    public class TokenBucketTests
    {
        [Fact]
        public async Task GetTicketAsync___should_allow_3_requests_in_5_seconds_with_TokenBucket_allowing_1_request_every_2_seconds()
        {
            var timeService = new AcceleratedTimeService(30);
            var bucket = new TokenBucket(1, 2, TimeUnit.Seconds, timeService);
            
            var spy = new List<DateTimeOffset>();

            var fiveSecondsLater = timeService.GetNow().AddSeconds(5);
            while (timeService.GetNow() < fiveSecondsLater)
            {
                await bucket.GetTicketAsync();
                spy.Add(timeService.GetNow());
            }

            var requestsInTime = spy.Where(s => s <= fiveSecondsLater).ToList();
            requestsInTime.Count.ShouldBe(3);
        }

        [Fact]
        public async Task GetTicketAsync___should_allow_10_requests_in_90_seconds_with_TokenBucket_allowing_5_requests_per_minute()
        {
            var timeService = new AcceleratedTimeService(500);
            var bucket = new TokenBucket(5, 1, TimeUnit.Minutes, timeService);

            var spy = new List<DateTimeOffset>();

            var ninetySecondsLater = timeService.GetNow().AddSeconds(90);
            while (timeService.GetNow() < ninetySecondsLater)
            {
                await bucket.GetTicketAsync();
                spy.Add(timeService.GetNow());
            }

            var requestsInTime = spy.Where(s => s <= ninetySecondsLater).ToList();
            requestsInTime.Count.ShouldBe(10);
        }

        [Fact]
        public async Task GetTicketAsync___should_allow_10_requests_in_90_minutes_with_TokenBucket_allowing_5_requests_per_hour()
        {
            var timeService = new AcceleratedTimeService(50000);
            var bucket = new TokenBucket(5, 1, TimeUnit.Hours, timeService);

            var spy = new List<DateTimeOffset>();

            var ninetyMinutesLater = timeService.GetNow().AddMinutes(90);
            while (timeService.GetNow() < ninetyMinutesLater)
            {
                await bucket.GetTicketAsync();
                spy.Add(timeService.GetNow());
            }

            var requestsInTime = spy.Where(s => s <= ninetyMinutesLater).ToList();
            requestsInTime.Count.ShouldBe(10);
        }
    }
}
