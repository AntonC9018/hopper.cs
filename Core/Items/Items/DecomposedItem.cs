namespace Hopper.Core.Items
{
    public class DecomposedItem
    {
        public readonly IItem item;
        public readonly int count;

        public DecomposedItem(IItem item)
        {
            this.item = item;
            this.count = 1;
        }

        public DecomposedItem(IItem item, int count)
        {
            this.item = item;
            this.count = count;
        }
    }
}