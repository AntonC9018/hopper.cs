using Core.FS;

namespace Core.Items
{
    public class PoolItem : File
    {
        public int id;
        public int quantity;

        public PoolItem(int id, int q)
        {
            this.id = id;
            this.quantity = q;
        }

        public PoolItem(IHaveId item, int q)
        {
            this.id = item.Id;
            this.quantity = q;
        }

        public override bool Equals(object obj)
        {
            return id == ((PoolItem)obj).id;
        }

        public override int GetHashCode()
        {
            return id;
        }
    }
}