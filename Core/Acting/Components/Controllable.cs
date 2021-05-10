using System.Collections.Generic;
using System.Reflection;
using Hopper.Utils.Vector;
using Hopper.Shared.Attributes;
using Hopper.Utils.Chains;
using Hopper.Core.Items;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;

namespace Hopper.Core.ActingNS
{
    public class InputMapping
    {
        public readonly static InputMapping Vector = new InputMapping("Vector");
        public readonly static InputMapping Action_0 = new InputMapping("Action_0");
        public readonly static InputMapping Action_1 = new InputMapping("Action_1");
        public readonly static InputMapping Special_0 = new InputMapping("Special_0");
        public readonly static InputMapping Special_1 = new InputMapping("Special_1");
        public readonly static InputMapping Item_0 = new InputMapping("Item_0");
        public readonly static InputMapping Item_1 = new InputMapping("Item_1");
        public readonly static InputMapping Weapon_0 = new InputMapping("Weapon_0");
        public readonly static InputMapping Weapon_1 = new InputMapping("Weapon_1");

        string name;
        public InputMapping(string name)
        {
            this.name = name;
        }

        public static IEnumerable<InputMapping> Members
        {
            get
            {
                var type = typeof(InputMapping);
                var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);

                foreach (var field in fields)
                    yield return (InputMapping)field.GetValue(null);
            }
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

        [Inject] public IAction defaultAction;
        public Dictionary<InputMapping, Chain<Context>> _chains;


        /// <summary>
        /// Gets the next action from the currently bound item slot, 
        /// then does a guard pass over a chain dedicated to that slot.
        /// </summary>
        public CompiledAction ConvertInputToAction(Entity entity, InputMapping input)
        {
            var ev = new Context { actor = entity };
            _chains[input].PassWithPropagationChecking(ev);
            return ev.Compile();
        }


        /// <summary>
        /// Gets the next action from the currently bound item slot, 
        /// then does a guard pass over a chain dedicated to that slot.
        /// </summary>
        public IAction ConvertSlotIdToAction(Entity entity, Identifier slotId)
        {
            var inventory = entity.GetInventory();
            if (inventory.TryGetItemFromSlot(slotId, out var item) 
                && item.TryGetItemAction(entity, out var action))
            {
                return action;
            }
            return null;
        }

        public CompiledAction ConvertVectorToAction(Entity entity, IntVector2 direction)
        {
            var ev = new Context
            {
                actor = entity,
                action = defaultAction,
                direction = direction
            };
            _chains[InputMapping.Vector].PassWithPropagationChecking(ev);
            return ev.Compile();
        }

        public static Dictionary<InputMapping, ChainPath<Chain<Context>>> Paths;
        static Controllable()
        {
            Paths = new Dictionary<InputMapping, ChainPath<Chain<Context>>>();
            // set up all chain paths for the input mappings
            // set up all templates
            foreach (InputMapping name in InputMapping.Members)
            {
                Paths[name] = new ChainPath<Chain<Context>>(
                    (Entity entity) => entity.TryGetComponent(Index, out var component) ? component._chains[name] : null);
            }
        }
    }
}