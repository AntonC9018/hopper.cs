using Chains;

namespace Core.Behaviors
{
    public class Tick : Behavior
    {
        public static string s_chainName = "tick";

        public class Event : EventBase
        {
            public Entity actor;
        }

        public static ChainPaths<Tick, Event> chain;
        static Tick()
        {
            var builder = new ChainTemplateBuilder();
            var tick = builder.AddTemplate<Event>(ChainName.Default);
            chain = new ChainPaths<Tick, Event>(ChainName.Default);
            BehaviorFactory<Tick>.s_builder = builder;
        }
    }
}