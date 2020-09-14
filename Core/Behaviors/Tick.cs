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
            chain = new ChainPaths<Tick, Event>(ChainName.Default);
            var builder = new ChainTemplateBuilder()
                .AddTemplate<Event>(ChainName.Default).End();
            BehaviorFactory<Tick>.s_builder = builder;
        }
    }
}