export interface Policy {
  requests: number;
  duration: number;
  units: "seconds" | "minutes" | "hours";
}

export const Policies = {
  TokenBucket(requests: number, duration: number, units: "seconds" | "minutes" | "hours"): Policy {
    return {
      requests,
      duration,
      units,
    };
  },
};
