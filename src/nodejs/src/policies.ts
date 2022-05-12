export interface Policy {}

export const Policies = {
  TokenBucket(
    requests: number,
    duration: number,
    units: "seconds" | "minutes" | "hours"
  ): Policy {
    return {};
  },
};
