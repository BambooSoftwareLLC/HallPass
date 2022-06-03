# HallPass - PRE-RELEASE
Client-side rate limiter library for NodeJS to help client-side developers respect the rate limits of the APIs that they consume.

## Installation
```
npm install HallPass
```

## Usage
Suppose you want to consume an API with the endpoint `https://api.foo.com/users`, which imposes a rate limit of **10 requests per minute**, implemented via a standard Token Bucket algorithm where you can burst 10 requests every minute.

To respect this rate limit in our application, we should create a single `HallMonitor` keyed to this endpoint with the correct `Policy`:
```
Policy policy = Policies.TokenBucket(requestsPerPeriod: 10, periodDurationMilliseconds: 1000 * 60);
HallMonitor usersMonitor = new LocalHallMonitor("https://api.foo.com/users", policy);
```

Then, anywhere in our application that needs to rate limit this particular API, we can use our `HallMonitor` like this:
```
const userId = 13;
const requestFunc = async () => requestFromFooAsync("https://api.foo.com/users/" + userId.toString());
FooResult result = await usersMonitor.request<FooResult>(requestFunc);
```

### Throttle a Single Call
To throttle a request anywhere in your application, just make sure you're using the same `HallMonitor` for a given endpoint at each place that you want to share the same rate limit.

```
// somewhere in your configuration
const policy = Policies.TokenBucket(requestsPerPeriod: 10, periodDurationMilliseconds: 1000 * 60);
const hallMonitor = new LocalHallMonitor("some/unique/uri/with/a/rate/limit", policy);

// somewhere in your application
const requestFunc = async () => requestFromFooAsync("https://api.foo.com/users/");
FooResult result = await hallMonitor.request<FooResult>(requestFunc);
```

### Throttle a Bunch of Calls in a Loop
HallPass works for synchronous loops (within async methods) by simply awaiting until it has permission to proceed based on the provided `Policy`.

```
const policy = Policies.TokenBucket(requestsPerPeriod: 10, periodDurationMilliseconds: 1000 * 60);
const hallMonitor = new LocalHallMonitor("some/unique/uri/with/a/rate/limit", policy);

const results: FooResult[] = [];

const idsToGet = Array(500).keys();
foreach (const id in idsToGet)
{
    const requestFunc = async () => requestFromFooAsync("https://api.foo.com/users/" + id.toString());
    FooResult result = await usersMonitor.request<FooResult>(requestFunc);
    results.push(result);
}
```

In this example, the first 10 requests would burst immediately, and then the 11th would be awaited for roughly _1 minute_, at which point it would burst 11-20... at which point it would wait roughly another minute, etc.

### COMING SOON: Throttle a Bunch of Calls Across Distributed Systems
Eventually, HallPass will be able to throttle calls across distributed systems. If you have multiple instances of an application running at once, but need to respect a single external API rate limit, or if you have multiple different applications running but still need to respect a single external API rate limit between all instances and applications, you'd be able to do so like this:

```
// in the configuration of each of your client instances/applications
const policy = Policies.TokenBucket(requestsPerPeriod: 10, periodDurationMilliseconds: 1000 * 60);
const hallMonitor = new DistributedHallMonitor("some/unique/uri/with/a/rate/limit", policy, "your-hallpass-app-key", "your-hallpass-app-secret");

// in your instances/applications
const requestFunc = async () => requestFromFooAsync("https://api.foo.com/users/");
FooResult result = await hallMonitor.request<FooResult>(requestFunc);
```

HallPass will take care of registering individual instances, "fairly" dolling out permissions, and tracking the global rate limit for your account/app and its usage on our servers.

HallPass will never know what endpoints you're calling, because the actual API call is still handled locally within each application. All that HallPass receives is an encrypted unique ID representing each `HallMonitor`'s unique key, and the policy used for that key.