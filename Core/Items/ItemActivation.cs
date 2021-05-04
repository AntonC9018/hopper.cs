using Hopper.Core.Components;
using Hopper.Shared.Attributes;

namespace Hopper.Core.Items
{
    public partial class SlotComponent : IComponent
    {
        [Inject] public Identifier slotId;
    }

    public partial class ItemActivation : IComponent
    {
        [Inject] public System.Func<Entity, Entity, Core.IAction> GetActionFunc;


        [Alias("GetItemAction")]
        public Core.IAction GetAction(Entity actor, Entity entityThatWillDoTheAction)
        {
            return GetActionFunc(actor, entityThatWillDoTheAction);
        }
    }   
}