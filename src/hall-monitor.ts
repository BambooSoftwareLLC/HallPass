export interface HallMonitor {
  request: <T>(action: () => Promise<T>) => Promise<T>;
}
