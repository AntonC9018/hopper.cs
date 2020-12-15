// this is the class that provides and lets you create and register all
// the kinds contained in the Core mod.
// It provides super essential stuff like
//      1. a base droppedItem factory
//      2. basic retouchers
//      3. stat registering
using Hopper.Core.Items;
using Hopper.Core.Mods;
using Hopper.Core.Registry;
using Hopper.Core.Retouchers;
using Hopper.Core.Stats.Basic;

namespace Hopper.Core
{
    public class CoreMod : IMod
    {
        public CoreMod()
        {
        }

        public string Name => "core";
        public int Offset => 1;

        public void RegisterSelf(ModSubRegistry registry)
        {
            CoreRetouchers.RegisterAll(registry);
            DroppedItem.Factory.RegisterSelf(registry);
            BasicSlots.RegisterSelf(registry);
        }

        public void Patch(Repository repository)
        {
            BasicSlots.Patch(repository);
            BasicStats.Patch(repository);
        }

        public void AfterPatch(Repository repository)
        {
            BasicStats.AfterPatch(repository);
        }
    }
}