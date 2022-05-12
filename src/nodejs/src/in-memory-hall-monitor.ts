import { HallMonitor } from "./hall-monitor";
import { Policy } from "./policies";

export class InMemoryHallMonitor implements HallMonitor {
  constructor(private key: string, private policy: Policy) {}

  async request<T>(action: () => Promise<T>): Promise<T> {
    return await action();
  }
}
