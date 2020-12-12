// this is the class that provides and lets you create and register all
// the kinds contained in the Core mod.
// It provides super essential stuff like
//      1. a base droppedItem factory
//      2. basic retouchers
//      3. stat registering
using Hopper.Core.Items;
using Hopper.Core.Mods;
using Hopper.Core.Retouchers;
using Hopper.Core.Stats.Basic;

namespace Hopper.Core
{
    public class CoreMod : IMod
    {
        public string Name => "core";
        public CoreRetouchers Retouchers;
        public EntityFactory<DroppedItem> DroppedItemFactory;

        public CoreMod(ModsContent mods)
        {
            Retouchers = new CoreRetouchers();
            DroppedItemFactory = new EntityFactory<DroppedItem>();
        }

        public void RegisterSelf(Registry registry)
        {
            BasicStats.Init(registry);
            
            Retouchers.RegisterAll(registry);
            DroppedItemFactory.RegisterSelf(registry);
        }
    }
}