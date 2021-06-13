namespace Hopper.Utils
{
    public struct AnonymousInt32Flags
    {
        public System.Int32 _value;

        public bool Get(int position)
        {
            return (_value & (1 << position)) != 0;
        }

        public void Set(int position)
        {
            _value |= (1 << position);
        }

        public void Unset(int position)
        {
            _value &= ~(1 << position);
        }
        
        public void Set(int position, bool set)
        {
            if (set) Set(position);
            else     Unset(position);
        }
    }
}