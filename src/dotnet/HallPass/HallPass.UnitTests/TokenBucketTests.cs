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
            var timeService = new TimeService();
            var taskDelayer = new TaskDelayer();
            var bucket = new TokenBucket(1, 2, TimeUnit.Seconds, timeService, taskDelayer);
            
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
    }
}
