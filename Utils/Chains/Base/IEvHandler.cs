namespace Hopper.Utils.Chains
{
    public interface IEvHandler<in Event> where Event : EventBase
    {
        int Priority { get; set; }
        void Call(Event ev);
    }
}