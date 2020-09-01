using Chains;

namespace Core
{
    public class Retoucher
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();
        public ChainDef[] chainDefinitions;

        public Retoucher(ChainDef[] chainDefinitions)
        {
            this.chainDefinitions = chainDefinitions;
        }

        public Retoucher(ChainDef chainDefinitions)
        {
            this.chainDefinitions = new ChainDef[] { chainDefinitions };
        }

        // beacuse I'm sick of boilerplate for simple stuff
        public static Retoucher SingleHandlered<T>(
            string name,
            System.Action<T> handler,
            PRIORITY_RANKS priority = PRIORITY_RANKS.MEDIUM)
            where T : EventBase
        {
            return new Retoucher(
                new ChainDef[]
                {
                    new ChainDef
                    {
                        name = name,
                        handlers = new IEvHandler[]
                        {
                            new EvHandler<T>(handler, priority)
                        }
                    }
                }
            );
        }
    }
}