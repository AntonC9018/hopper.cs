using Hopper.Core.Components;
using Hopper.Shared.Attributes;

namespace Hopper.Core.Items
{
    [AutoActivation("ItemActivate")]
    public partial class ItemActivation : IBehavior
    {
        [Inject] public Identifier slotId;
        public class Context : ActorContext
        {
        }
    }   
}