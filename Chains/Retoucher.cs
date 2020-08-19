namespace Core
{
    public class Retoucher
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();
        public ChainDefinition[] chainDefinitions;

        public Retoucher(ChainDefinition chainDefinition)
        {
            chainDefinitions = new ChainDefinition[] { chainDefinition };
        }

        public Retoucher(ChainDefinition[] chainDefinitions)
        {
            this.chainDefinitions = chainDefinitions;
        }
    }
}