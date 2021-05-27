namespace Hopper.Core.Components
{
    public class Index<T>
    {
        public Identifier Id;

        public override bool Equals(object obj)
        {
            return Id.Equals((obj as Index<T>).Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"Index<{typeof(T).Name}>({Id.ToString()})";
        }

        public static bool operator ==(Index<T> index1, Index<T> index2)
        {
            return index1.Id == index2.Id;
        }

        public static bool operator !=(Index<T> index1, Index<T> index2)
        {
            return index1.Id != index2.Id;
        }
    }
}