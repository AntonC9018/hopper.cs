using Hopper.Core.Registries;
using Hopper.Utils.Chains;
using Hopper.Core.Chains;
using Hopper.Core.Behaviors;

namespace Hopper.Core
{
    public class Retoucher : Kind<Retoucher>
    {
        private ITemplateChainDef[] m_chainDefinitions;

        public Retoucher(ITemplateChainDef[] chainDefinitions)
        {
            this.m_chainDefinitions = chainDefinitions;
        }

        // beacuse I'm sick of boilerplate for simple stuff
        public static Retoucher SingleHandlered<T>(
            IChainPaths<T> path,
            System.Action<T> handler,
            PriorityRank priority = PriorityRank.Default)
            where T : EventBase
        {
            return new Retoucher(
                new ITemplateChainDef[]
                {
                    new TemplateChainDef<T>
                    {
                        path = path.TemplatePath,
                        handlers = new Stuff<T>[]
                        {
                            new Stuff<T> {handler = handler, priority = (int)priority }
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