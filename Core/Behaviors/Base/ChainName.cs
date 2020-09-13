namespace Core.Behaviors
{
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

        public readonly static ChainName Check = NextNew();
        public readonly static ChainName Do = NextNew();
        public readonly static ChainName Success = NextNew();
        public readonly static ChainName Fail = NextNew();
        public readonly static ChainName Condition = NextNew();
        public readonly static ChainName Default = NextNew();
    }
}