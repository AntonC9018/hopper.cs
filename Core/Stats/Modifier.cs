using Utils;

namespace Core.Stats
{
    public class Modifier
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();
        public IStatPath<StatFile> path;

        public override int GetHashCode()
        {
            return id;
        }
    }

    public class StatModifier : Modifier
    {
        public StatFile file;
        public StatModifier(IStatPath<StatFile> path, StatFile file)
        {
            this.file = file;
            base.path = path;
        }
    }

    public class ChainModifier : Modifier
    {
        public Chains.EvHandler<StatEvent> handler;
        public ChainModifier(IStatPath<StatFile> path, Chains.EvHandler<StatEvent> handler)
        {
            this.handler = handler;
            base.path = path;
        }
    }
}