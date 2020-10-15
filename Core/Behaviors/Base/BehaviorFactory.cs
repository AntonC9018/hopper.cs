using Chains;

namespace Core.Behaviors
{
    public class BehaviorFactory<Beh> : BehaviorFactory, IProvidesChainTemplate
           where Beh : Behavior, new()
    {
        public static ChainTemplateBuilder s_builder;

        static BehaviorFactory()
        {
            var type = typeof(Beh);
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
        }
        public BehaviorFactory()
        {
            m_templates = s_builder?.Templates;
        }

        public ChainTemplate<Event> GetTemplate<Event>(ChainName name) where Event : EventBase
        {
            return (ChainTemplate<Event>)m_templates[name];
        }

        public override Behavior Instantiate(Entity entity, BehaviorConfig conf)
        {
            Behavior behavior = new Beh();
            behavior.GenerateChains(m_templates);
            behavior.Init(entity, conf);
            return behavior;
        }

    }
}