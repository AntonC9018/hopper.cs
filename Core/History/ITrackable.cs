namespace Core.History
{
    public interface ITrackable<out T>
    {
        T GetState();
        World World { get; }
    }
}