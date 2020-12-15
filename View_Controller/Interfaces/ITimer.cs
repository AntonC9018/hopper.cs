namespace Hopper.Controller
{
    public interface ITimer
    {
        void Start(); // starts the timer from zero
        void Resume(); // continues the timer from the amount it left off
        void Stop(); // resets the timer to zero and stops it
        void Pause(); // stops the timer without resetting the timer
        void Reset(); // resets the timer
        void Set(int millis); // sets the timer
        event System.Action<int> TimerEvent;
    }
}