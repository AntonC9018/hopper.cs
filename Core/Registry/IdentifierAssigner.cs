namespace Hopper.Core
{
    public class IdentifierAssigner
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