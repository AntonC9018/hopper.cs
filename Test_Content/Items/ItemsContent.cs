using Hopper.Core;
using Hopper.Core.Items;
using Hopper.Core.Retouchers;

namespace Hopper.Test_Content.Boss
{
    public class ItemsContent
    {
        public ModularItem DefaultBow;
        // public ModularItem FrontShield;

        public ItemsContent()
        {
            DefaultBow = Bow.CreateBow();
            // FrontShield = new ModularItem(new ItemMetadata("Front_Shield"), 
        }

        public void RegisterSelf(Registry registry)
        {
            Bow.ArrowSource.RegisterOn(registry);
            DefaultBow.RegisterSelf(registry);
        }
    }
}