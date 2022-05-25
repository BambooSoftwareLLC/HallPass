export class TokenBucket {
  private currentCapacity: number = 0;
  private lastRefillMilliseconds: number = new Date("2000-01-01").getTime();
  private periodDurationMilliseconds: number;

  constructor(
    private requestsPerPeriod: number,
    private periodDuration: number,
    private periodUnits: "seconds" | "minutes" | "hours"
  ) {
    this.periodDurationMilliseconds = periodDuration * 1000;
  }

  public async getTicket() {
    // todo

    // check if capacity available
    //  - if yes, resolve
    if (this.currentCapacity > 0) {
      this.currentCapacity--;
      return;
    }

    //  - if no, see if we can get more capacity
    //  - if can't get immediate capacity, wait until we can
    await this.refill();
    await this.getTicket();
  }

  private async refill(): Promise<void> {
    const now = new Date();
    const waitTime = this.lastRefillMilliseconds + this.periodDurationMilliseconds - now.getTime();
    if (waitTime <= 0) {
      this.lastRefillMilliseconds = now.getTime();
      this.currentCapacity = this.requestsPerPeriod;
      return;
    } else {
      return new Promise(async (resolve, reject) => {
        setTimeout(async () => {
          await this.refill();
          resolve();
        }, waitTime);
      });
    }
  }
}
