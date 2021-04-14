namespace Hopper.Core
{
    public struct Identifier 
    {
        public int mod_number;
        public int number;

        public Identifier(int mod_number, int number)
        {
            this.mod_number = mod_number;
            this.number = number;
        }

        public override bool Equals(object obj)
        {
            return obj is Identifier identifier &&
                   mod_number == identifier.mod_number &&
                   number == identifier.number;
        }

        public override int GetHashCode() => (mod_number << 16) | number;
    }
}