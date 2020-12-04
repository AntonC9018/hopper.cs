using Core.FS;

namespace Core.Items
{
    public class PoolItem : File
    {
        public int id;
        public int quantity;

        public PoolItem(int id, int quantity)
        {
            this.id = id;
            this.quantity = quantity;
        }

        public PoolItem(IHaveId item, int quantity)
        {
            this.id = item.Id;
            this.quantity = quantity;
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