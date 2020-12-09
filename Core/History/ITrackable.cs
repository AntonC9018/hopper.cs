namespace Hopper.Core.History
{
    public interface ITrackable<out T>
    {
        T GetState();
    }
}