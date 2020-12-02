using System;
using Chains;
using Core.Behaviors;
using Core.Utils;

namespace Core
{
    public class Retoucher : IHaveId
    {
        public int Id => m_id;
        public readonly int m_id;
        private ITemplateChainDef[] m_chainDefinitions;

        public Retoucher(ITemplateChainDef[] chainDefinitions)
        {
            this.m_chainDefinitions = chainDefinitions;
            m_id = Registry.Default.Retoucher.Add(this);
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

        public void Retouch(IProvideBehaviorFactory entityFactory)
        {
            foreach (var def in m_chainDefinitions)
            {
                def.AddHandlersTo(entityFactory);
            }
        }
    }
}