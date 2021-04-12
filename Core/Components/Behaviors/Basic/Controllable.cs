using System.Collections.Generic;
using System.Reflection;
using Hopper.Utils.Vector;
using System.Runtime.Serialization;
using Hopper.Shared.Attributes;
using Hopper.Utils.Chains;

namespace Hopper.Core.Components.Basic
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
            public Action action = null;
            public IntVector2 direction;

            public ParticularAction ToParticular()
            {
                if (action == null)
                {
                    return null;
                }
                if (action is DirectedAction)
                {
                    return ((DirectedAction)action).ToDirectedParticular(direction);
                }
                return action.ToParticular();
            }
        }

        [Inject] public Action defaultAction;
        public Dictionary<InputMapping, Chain<Context>> _chains;


        public ParticularAction ConvertInputToAction(Entity entity, InputMapping input)
        {
            var ev = new Context { actor = entity };
            _chains[input].Pass(ev);
            return ev.ToParticular();
        }

        public ParticularAction ConvertVectorToAction(Entity entity, IntVector2 direction)
        {
            var ev = new Context
            {
                actor = entity,
                action = defaultAction,
                direction = direction
            };
            _chains[InputMapping.Vector].Pass(ev);
            return ev.ToParticular();
        }

        public static Dictionary<InputMapping, Path<Chain<Context>>> Paths;
        static Controllable()
        {
            // set up all chain paths for the input mappings
            // set up all templates
            foreach (InputMapping name in InputMapping.Members)
            {
                Paths[name] = new Path<Chain<Context>>
                {
                    Chain = (Entity entity) => entity.GetComponent(Index)._chains[name] 
                };
            }
        }
    }
}