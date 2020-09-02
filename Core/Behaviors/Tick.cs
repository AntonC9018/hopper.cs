using Chains;

namespace Core.Behaviors
{
    public static class Tick
    {
        public static string s_chainName = "tick";

        public class Event : EventBase
        {
            public Entity actor;
        }
    }
}