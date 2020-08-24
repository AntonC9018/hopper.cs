namespace Core
{
    public class Retoucher
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();
        public IChainDef[] chainDefinitions;

        public Retoucher(IChainDef chainDefinition)
        {
            chainDefinitions = new IChainDef[] { chainDefinition };
        }

        public Retoucher(IChainDef[] chainDefinitions)
        {
            this.chainDefinitions = chainDefinitions;
        }
    }
}