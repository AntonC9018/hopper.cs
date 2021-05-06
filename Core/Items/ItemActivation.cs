using Hopper.Core.ActingNS;
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
        [Inject] public System.Func<Entity, Entity, IAction> GetActionFunc;


        [Alias("GetItemAction")]
        public IAction GetAction(Entity actor, Entity entityThatWillDoTheAction)
        {
            return GetActionFunc(actor, entityThatWillDoTheAction);
        }
    }   
}