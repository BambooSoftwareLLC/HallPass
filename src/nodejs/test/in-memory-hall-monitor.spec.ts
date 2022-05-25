import { InMemoryHallMonitor } from "../src/in-memory-hall-monitor";
import { Policies } from "../src/policies";

describe("InMemoryHallMonitor", () => {
  it("should allow 3 requests in 4 seconds with TokenBucket allowing 1 request every 2 seconds", async () => {
    const hallMonitor = new InMemoryHallMonitor("foo", Policies.TokenBucket(1, 2, "seconds"));

    // mock a target that gets updated when a request is made, so we can track them
    const spy = { requests: 0 };
    const updateSpy = async () => spy.requests++;
    const requests = [1, 2, 3, 4, 5, 6, 7, 8, 9].map((_) => updateSpy());

    expect(spy.requests).toBe(9);

    // in a loop, try to make a bunch of requests
    spy.requests = 0;

    let fourSecondsLater = new Date();
    fourSecondsLater.setMilliseconds(fourSecondsLater.getMilliseconds() + 3900);
    while (new Date() < fourSecondsLater) {
      await hallMonitor.request(updateSpy);
    }

    // run this for 5 seconds and assert that we've made 3 requests
    expect(spy.requests).toBe(3);
  }, 10000);
});
