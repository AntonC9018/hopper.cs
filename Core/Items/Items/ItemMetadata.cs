namespace Core.Items
{
    // This class serves for ease of debugging and for view applications only.
    // It has nothing to do with the logic part of the application.
    // This is why `slot` and `id` is not defined here.
    // They actually used in the logic part, while the name isn't.
    // This may also include the name of the mod that's registering the item. 
    public class ItemMetadata
    {
        public string name;

        public ItemMetadata(string name)
        {
            this.name = name;
        }
    }
}