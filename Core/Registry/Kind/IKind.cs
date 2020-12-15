namespace Hopper.Core.Registry
{
    /*
        `Kind` basically means static content that gets assigned an id by the Kind Registry.
        
        Kinds are instances of specific classes. 
        They are expected to get instantiated once per game.
        They are slightly different from singletons in the sense that there is 
        not necessarily just one instance of that specific type per program execution.
        
        For example, items are kinds. 
        Suppose we had a shovel item and a pickaxe item.
        They are both instances of the `Item` class, but both the shovel and the pickaxe
        would be instantiated once per program run. In this sense, the item class
        conceptually represents the logic associated with that item: what effects it
        gives, what stats it up etc. The actual dropped or drawn to the screen item
        is a totally different and basically unrelated thing.

        The term `Kind` was selected, because `Type` already has an established meaning.

        Kinds are opposed to Instances, which are expected to be reinstantiated, 
        while also having id's.
    */
    public interface IKind : IHaveId
    {
        void RegisterSelf(ModRegistry registry);
    }
}