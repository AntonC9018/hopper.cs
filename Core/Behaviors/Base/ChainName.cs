using Utils;

namespace Core.Behaviors
{
    // https://stackoverflow.com/a/2807561
    public struct ChainName
    {
        static IdGenerator s_idGenerator = new IdGenerator();
        readonly int value;
        public ChainName(int value)
        {
            this.value = value;
        }
        public static implicit operator int(ChainName sport)
        {
            return sport.value;
        }
        public static implicit operator ChainName(int sport)
        {
            return new ChainName(sport);
        }
        public static int NextNew()
        {
            return s_idGenerator.GetNextId();
        }
        public override int GetHashCode()
        {
            return value;
        }
        public override bool Equals(object obj)
        {
            return value == ((ChainName)obj).value;
        }

        public readonly static ChainName Check = NextNew();
        public readonly static ChainName Do = NextNew();
        public readonly static ChainName Success = NextNew();
        public readonly static ChainName Fail = NextNew();
        public readonly static ChainName Condition = NextNew();
        public readonly static ChainName Default = NextNew();
    }
}