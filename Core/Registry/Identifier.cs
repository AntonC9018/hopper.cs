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
    }
}