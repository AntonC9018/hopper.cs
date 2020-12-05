using System.Collections.Generic;
using System.Reflection;
using Hopper.Core.Chains;
using Hopper.Utils.Chains;

namespace Hopper.Core.Behaviors
{
    public class BehaviorFactory<Beh> : IBehaviorFactory, IProvidesChainTemplate
           where Beh : Behavior, new()
    {
        protected Dictionary<ChainName, IChainTemplate> m_templates =
            new Dictionary<ChainName, IChainTemplate>();
        public static ChainTemplateBuilder s_builder;
        private static MethodInfo InitMethodInfo;

        static BehaviorFactory()
        {
            var type = typeof(Beh);
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            InitMethodInfo = type.GetMethod("Init", BindingFlags.Instance | BindingFlags.NonPublic);
        }
        public BehaviorFactory()
        {
            m_templates = s_builder?.Templates;
        }

        public ChainTemplate<Event> GetTemplate<Event>(ChainName name) where Event : EventBase
        {
            return (ChainTemplate<Event>)m_templates[name];
        }

        public Behavior Instantiate(Entity entity, object conf)
        {
            Behavior behavior = new Beh();
            behavior._SetEntity(entity);
            behavior.GenerateChains(m_templates);
            InitMethodInfo?.Invoke(behavior, new object[] { conf });
            return behavior;
        }
    }
}