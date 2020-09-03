using System;
using Chains;

namespace Core
{
    public class Retoucher
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();
        IChainDef[] m_chainDefinitions;

        public Retoucher(IChainDef[] chainDefinitions)
        {
            this.m_chainDefinitions = chainDefinitions;
        }

        public Retoucher(IChainDef chainDefinitions)
        {
            this.m_chainDefinitions = new IChainDef[] { chainDefinitions };
        }

        // beacuse I'm sick of boilerplate for simple stuff
        public static Retoucher SingleHandlered<T>(
            System.Func<IProvideBehavior, ICanAddHandlers<T>> path,
            System.Action<T> handler,
            PRIORITY_RANKS priority = PRIORITY_RANKS.DEFAULT)
            where T : EventBase
        {
            return new Retoucher(
                new IChainDef[]
                {
                    new IChainDef<T>
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

        internal void Retouch(IProvideBehavior entityFactory)
        {
            foreach (var def in m_chainDefinitions)
            {
                def.AddHandlersTo(entityFactory);
            }
        }
    }
}