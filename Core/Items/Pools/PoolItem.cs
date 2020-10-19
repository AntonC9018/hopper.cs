namespace Core.Items
{
    public class PoolItem
    {
        public int id;
        public int q;

        public PoolItem(int id, int q)
        {
            this.id = id;
            this.q = q;
        }

        public PoolItem Copy()
        {
            return (PoolItem)this.MemberwiseClone();
        }
    }
}