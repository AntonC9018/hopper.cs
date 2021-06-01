namespace Hopper.Core
{
    public struct IdentifierAssigner
    {
        public int offset;

        public int Next()
        {
            return ++offset;
        }

        public Identifier NextIdentifierForCurrentMod()
        {
            return new Identifier(Registry.Global._currentMod, Next());
        }
    }
}