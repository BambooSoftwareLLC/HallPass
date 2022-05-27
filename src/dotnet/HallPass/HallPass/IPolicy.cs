namespace HallPass
{
    public interface IPolicy
    {
        int Requests { get; }
        int Duration { get; }
        TimeUnit TimeUnit { get; }
    }
}
