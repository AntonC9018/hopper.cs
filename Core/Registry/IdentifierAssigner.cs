namespace Hopper.Core
{
    public struct IdentifierAssigner
    {
        public int offset;

        public int Next()
        {
            return ++offset;
        }
    }
}