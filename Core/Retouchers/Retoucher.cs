using System;
using Chains;
using Core.Behaviors;

namespace Core
{
    public class Retoucher
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();
        ITemplateChainDef[] m_chainDefinitions;

        public Retoucher(ITemplateChainDef[] chainDefinitions)
        {
            this.m_chainDefinitions = chainDefinitions;
        }

        public Retoucher(ITemplateChainDef chainDefinitions)
        {
            this.m_chainDefinitions = new ITemplateChainDef[] { chainDefinitions };
        }

        // beacuse I'm sick of boilerplate for simple stuff
        public static Retoucher SingleHandlered<T>(
            BehaviorFactoryPath<T> path,
            System.Action<T> handler,
            PRIORITY_RANKS priority = PRIORITY_RANKS.DEFAULT)
            where T : EventBase
        {
            return new Retoucher(
                new ITemplateChainDef[]
                {
                    new TemplateChainDef<T>
                    {
                        path = path,
                        handlers = new EvHandler<T>[]
                        {
                            new EvHandler<T>(handler, priority)
                        }
                    }
                }
            );
        }

        internal void Retouch(IProvideBehaviorFactory entityFactory)
        {
            foreach (var def in m_chainDefinitions)
            {
                def.AddHandlersTo(entityFactory);
            }
        }
    }
}