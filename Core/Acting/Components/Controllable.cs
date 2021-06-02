using System.Collections.Generic;
using Hopper.Utils.Vector;
using Hopper.Shared.Attributes;
using Hopper.Utils.Chains;
using Hopper.Core.Items;
using Hopper.Core.Components;
using Hopper.Core.WorldNS;
using Hopper.Core.Components.Basic;

namespace Hopper.Core.ActingNS
{
    public struct InputMapping
    {
        public static IdentifierAssigner assigner = new IdentifierAssigner(); 
        
        // TODO: 
        // the thing I'm doing here is garbage:
        // 1. Mods cannot add new ones.
        // 2. These should be stored in the registry.
        // 3. I want to allow other code to store custom stuff that needs ids in the registry too,
        //    for that we need to generate more generic code.
        //    My current idea is to use an attibute, which would take a string or something 
        //    as the target registry, the registry itself being either defined in the custom
        //    subregistry per mod, or be in the default registry.
        //    Since this is in Core, the registry is assumed to already contain the subregistry.
        //    The code may then inherit that generic attribute.
        // say 
        // [ExportedToRegistry("Registry.Name")]
        //  RegistryNameExportAttribute : ExportedToRegstry { cctor() : base("Registry.Name") {} }
        // [RegistryNameExportAttribute]
        public readonly static InputMapping Vector = new InputMapping();
        public readonly static InputMapping Special_0 = new InputMapping();
        public readonly static InputMapping Special_1 = new InputMapping();

        // TODO: When an activated slot is created, it should automatically create a mapping? 
        public readonly static InputMapping Item_0 = new InputMapping();
        public readonly static InputMapping Item_1 = new InputMapping();
        
        public readonly int id;

        public InputMapping(int id)
        {
            this.id = assigner.Next();
        }
    }

    public partial class Controllable : IComponent
    {
        public class Context : ActorContext
        {
            public IAction action = null;
            public IntVector2 direction;

            public CompiledAction Compile()
            {
                return new CompiledAction(action, direction);
            }
        }

        [Inject] public IAction defaultVectorAction;
        public Dictionary<InputMapping, Chain<Context>> _chains;


        /// <summary>
        /// Currently, does nothing.
        /// What is should actually do, is it should return the special action assigned to the given input.
        /// </summary>
        public CompiledAction ConvertInputToAction(Entity entity, InputMapping input)
        {
            Hopper.Utils.Assert.That(false);
            return default;
        }

        /// <summary>
        /// Gets the next action from the currently bound item slot. 
        /// Then does a guard pass over a chain dedicated to that slot (unimplemented).
        /// </summary>
        private IAction ConvertSlotIdToAction(Entity entity, Identifier slotId)
        {
            var inventory = entity.GetInventory();
            if (inventory.TryGetItemFromSlot(slotId, out var item) 
                && item.TryGetItemAction(entity, out var action))
            {
                return action;
            }
            return null;
        } 

        /// <summary>
        /// </summary>
        public void SelectActionByItemSlot(Entity entity, Identifier slotId)
        {
            var action = ConvertSlotIdToAction(entity, slotId)
                .Compile(entity.GetTransform().orientation);

            entity.GetActing().SetPotentialAction(action);
        }

        /// <summary>
        /// Sets the default vector action for the given direction as the next action.
        /// The action may get substituted with a different action, e.g. if the character is sliding.
        /// </summary>
        public void SelectVectorAction(Entity entity, IntVector2 direction)
        {
            var action = defaultVectorAction.Compile(direction);

            // Runs the ActionSelected event, so it may be saved as a different action in the end.
            entity.GetActing().SetPotentialAction(action);
        }
    }
}