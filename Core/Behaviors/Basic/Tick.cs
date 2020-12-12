using System.Runtime.Serialization;
using Hopper.Core.Chains;

namespace Hopper.Core.Behaviors.Basic
{
    [DataContract]
    public class Tick : Behavior
    {
        public class Event : ActorEvent
        {
        }

        public void Activate()
        {
            var ev = new Event { actor = m_entity };
            GetChain<Event>(ChainName.Default).Pass(ev);
        }

        public static readonly ChainPaths<Tick, Event> Chain;
        static Tick()
        {
            Chain = new ChainPaths<Tick, Event>(ChainName.Default);
            var builder = new ChainTemplateBuilder()
                .AddTemplate<Event>(ChainName.Default).End();
            BehaviorFactory<Tick>.s_builder = builder;
        }
    }
}