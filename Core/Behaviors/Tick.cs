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

        public static ChainPath<Tick, Event> chain;
        public Tick(Entity entity)
        {

        }
        static Tick()
        {
            var builder = new ChainTemplateBuilder();
            var tick = builder.AddTemplate<Event>(s_chainName);
            chain = new ChainPath<Tick, Event>(s_chainName);
            BehaviorFactory<Tick>.s_builder = builder;
        }
    }
}