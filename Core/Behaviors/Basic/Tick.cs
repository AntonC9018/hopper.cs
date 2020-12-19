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

        public static readonly ChainTemplateBuilder DefaultBuilder;
        public static ConfiglessBehaviorFactory<Tick> Preset =>
            new ConfiglessBehaviorFactory<Tick>(DefaultBuilder);

        static Tick()
        {
            Chain = new ChainPaths<Tick, Event>(ChainName.Default);
            DefaultBuilder = new ChainTemplateBuilder()
                .AddTemplate<Event>(ChainName.Default).End();
        }
    }
}