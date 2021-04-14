namespace Hopper.Core
{
    public struct RuntimeIdentifier
    {
        public int number;

        public RuntimeIdentifier(int number)
        {
            this.number = number;
        }

        public override bool Equals(object obj)
        {
            return obj is RuntimeIdentifier id &&
                   number == id.number;
        }

        public override int GetHashCode() => number;
    }
}