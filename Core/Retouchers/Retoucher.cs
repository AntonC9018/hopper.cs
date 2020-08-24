namespace Core
{
    public class Retoucher
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();
        public ChainDef<CommonEvent>[] chainDefinitions;

        public Retoucher(ChainDef<CommonEvent> chainDefinition)
        {
            chainDefinitions = new ChainDef<CommonEvent>[] { chainDefinition };
        }

        public Retoucher(ChainDef<CommonEvent>[] chainDefinitions)
        {
            this.chainDefinitions = chainDefinitions;
        }
    }
}