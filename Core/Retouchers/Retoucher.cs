using Hopper.Core.Registries;
using Hopper.Utils.Chains;
using Hopper.Core.Chains;
using Hopper.Core.Behaviors;

namespace Hopper.Core
{
    public class Retoucher : IKind
    {
        public int Id => m_id;
        private int m_id;
        private ITemplateChainDef[] m_chainDefinitions;

        public Retoucher(ITemplateChainDef[] chainDefinitions)
        {
            this.m_chainDefinitions = chainDefinitions;
        }

        public void RegisterSelf(ModRegistry registry)
        {
            m_id = registry.Add<Retoucher>(this);
        }

        // beacuse I'm sick of boilerplate for simple stuff
        public static Retoucher SingleHandlered<T>(
            IChainPaths<T> path,
            System.Action<T> handler,
            PriorityRanks priority = PriorityRanks.Default)
            where T : EventBase
        {
            return new Retoucher(
                new ITemplateChainDef[]
                {
                    new TemplateChainDef<T>
                    {
                        path = path.TemplatePath,
                        handlers = new EvHandler<T>[]
                        {
                            new EvHandler<T>(handler, priority)
                        }
                    }
                }
            );
        }

        public void Retouch(IProvideBehaviorFactory EntityFactory)
        {
            foreach (var def in m_chainDefinitions)
            {
                def.AddHandlersTo(EntityFactory);
            }
        }
    }
}