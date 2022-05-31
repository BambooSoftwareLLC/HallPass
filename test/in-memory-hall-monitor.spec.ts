import { InMemoryHallMonitor } from "../src/in-memory-hall-monitor";
import { Policies } from "../src/policies";
import { resetTime, scaleTime } from "scaled-time";

describe("InMemoryHallMonitor", () => {
  afterEach(() => resetTime());

  it("should allow 3 requests in 5 seconds with TokenBucket allowing 1 request every 2 seconds", async () => {
    scaleTime(50);

    const hallMonitor = new InMemoryHallMonitor("foo", Policies.TokenBucket(1, 2, "seconds"));

    // mock a target that gets updated when a request is made, so we can track them
    const spy = { requests: [] as Date[] };
    const updateSpy = async () => spy.requests.push(new Date());
    const requests = [1, 2, 3, 4, 5, 6, 7, 8, 9].map((_) => updateSpy());

    expect(spy.requests.length).toBe(9);

    // in a loop, try to make a bunch of requests
    spy.requests = [];

    let fiveSecondsLater = new Date();
    fiveSecondsLater.setSeconds(fiveSecondsLater.getSeconds() + 5);

    await hallMonitor.request(updateSpy);

    while (new Date() < fiveSecondsLater) {
      await hallMonitor.request(updateSpy);
    }

    // run this for 5 seconds and assert that we've made 3 requests
    const requestsInTime = spy.requests.filter((d) => d <= fiveSecondsLater);
    expect(requestsInTime.length).toBe(3);
  });

  it("should allow 10 requests in 90 seconds with TokenBucket allowing 5 requests every minute", async () => {
    scaleTime(2000);

    const hallMonitor = new InMemoryHallMonitor("foo", Policies.TokenBucket(5, 1, "minutes"));

    // mock a target that gets updated when a request is made, so we can track them
    const spy = { requests: [] as Date[] };
    const updateSpy = async () => spy.requests.push(new Date());
    const requests = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15].map((_) => updateSpy());

    expect(spy.requests.length).toBe(15);

    // in a loop, try to make a bunch of requests
    spy.requests = [];

    let ninetySecondsLater = new Date();
    ninetySecondsLater.setSeconds(ninetySecondsLater.getSeconds() + 90);

    await hallMonitor.request(updateSpy);

    while (new Date() < ninetySecondsLater) {
      await hallMonitor.request(updateSpy);
    }

    // run this for 90 seconds and assert that we've made 10 requests
    const requestsInTime = spy.requests.filter((d) => d <= ninetySecondsLater);
    expect(requestsInTime.length).toBe(10);
  });

  it("should allow 10 requests in 90 minutes with TokenBucket allowing 5 requests every hour", async () => {
    scaleTime(20000);

    const hallMonitor = new InMemoryHallMonitor("foo", Policies.TokenBucket(5, 1, "hours"));

    // mock a target that gets updated when a request is made, so we can track them
    const spy = { requests: [] as Date[] };
    const updateSpy = async () => spy.requests.push(new Date());
    const requests = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15].map((_) => updateSpy());

    expect(spy.requests.length).toBe(15);

    // in a loop, try to make a bunch of requests
    spy.requests = [];

    let ninetyMinutesLater = new Date();
    ninetyMinutesLater.setMinutes(ninetyMinutesLater.getMinutes() + 90);

    await hallMonitor.request(updateSpy);

    while (new Date() < ninetyMinutesLater) {
      await hallMonitor.request(updateSpy);
    }

    // run this for 90 seconds and assert that we've made 10 requests
    const requestsInTime = spy.requests.filter((d) => d <= ninetyMinutesLater);
    expect(requestsInTime.length).toBe(10);
  });
});
