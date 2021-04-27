using Hopper.Core.Components;
using Hopper.Shared.Attributes;

namespace Hopper.Core.Items
{
    public partial class Countable : IComponent
    {
        public int count = 1;

        public void AbsorbItem(Entity item)
        {
            // the item will be lost
            // So, remove it from the registry
            Registry.Global.UnregisterRuntimeEntity(item);

            var countable = item.GetCountable();
            count += countable.count;
        }
    }   
}