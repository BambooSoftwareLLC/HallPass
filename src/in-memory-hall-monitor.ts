import { HallMonitor } from "./hall-monitor";
import { Policy } from "./policies";
import { TokenBucket } from "./token-bucket";

export class InMemoryHallMonitor implements HallMonitor {
  private bucket: TokenBucket;

  constructor(private key: string, private policy: Policy) {
    this.bucket = new TokenBucket(this.policy.requests, this.policy.duration, this.policy.units);
  }

  async request<T>(action: () => Promise<T>): Promise<T> {
    await this.bucket.getTicket();
    return await action();
  }
}
