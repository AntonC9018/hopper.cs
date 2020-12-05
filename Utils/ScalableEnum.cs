namespace Hopper.Utils
{
    public class ScalableEnum
    {
        public string Name { get; private set; }
        protected int m_value;

        public ScalableEnum(string name, int value)
        {
            m_value = value;
            Name = name;
        }

        public static implicit operator int(ScalableEnum myEnum)
        {
            return myEnum.m_value;
        }

        public override int GetHashCode()
        {
            return m_value;
        }

        public override bool Equals(object obj)
        {
            return m_value == ((ScalableEnum)obj).m_value;
        }

        public override string ToString()
        {
            return $"{Name}<{m_value}>";
        }
    }
}