using Hopper.Core.Components;
using Hopper.Shared.Attributes;

namespace Hopper.Core.Items
{
    public partial class SlotComponent : IComponent
    {
        [Inject] public Identifier slotId;
    }

    [AutoActivation("ItemActivate")]
    public partial class ItemActivation : IBehavior
    {
        public class Context : ActorContext
        {
        }
    }   
}