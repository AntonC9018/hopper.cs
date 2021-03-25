using System.Collections.Generic;
using System.Reflection;
using Hopper.Utils.Vector;
using System.Runtime.Serialization;
using Hopper.Core.Chains;

namespace Hopper.Core.Components.Basic
{
    public class InputMapping : ChainName
    {
        // public readonly static InputMapping Up = new InputMapping("Up");
        // public readonly static InputMapping Down = new InputMapping("Down");
        // public readonly static InputMapping Left = new InputMapping("Left");
        // public readonly static InputMapping Right = new InputMapping("Right");
        public readonly static InputMapping Vector = new InputMapping("Vector");
        public readonly static InputMapping Action_0 = new InputMapping("Action_0");
        public readonly static InputMapping Action_1 = new InputMapping("Action_1");
        public readonly static InputMapping Special_0 = new InputMapping("Special_0");
        public readonly static InputMapping Special_1 = new InputMapping("Special_1");
        public readonly static InputMapping Item_0 = new InputMapping("Item_0");
        public readonly static InputMapping Item_1 = new InputMapping("Item_1");
        public readonly static InputMapping Weapon_0 = new InputMapping("Weapon_0");
        public readonly static InputMapping Weapon_1 = new InputMapping("Weapon_1");

        public InputMapping(string name) : base(name)
        {
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

    [DataContract]
    public class Controllable : Behavior, IInitable<Action>
    {
        public class Event : ActorEvent
        {
            public Action action;
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

        public Action config_defaultAction;

        public void Init(Action defaultAction)
        {
            config_defaultAction = defaultAction;
        }

        public ParticularAction ConvertInputToAction(InputMapping input)
        {
            var ev = new Event { actor = m_entity };
            GetChain<Event>(input).Pass(ev);
            return ev.ToParticular();
        }

        public ParticularAction ConvertVectorToAction(IntVector2 direction)
        {
            var ev = new Event
            {
                actor = m_entity,
                action = config_defaultAction,
                direction = direction
            };
            GetChain<Event>(InputMapping.Vector).Pass(ev);
            return ev.ToParticular();
        }

        public static readonly Dictionary<InputMapping, ChainPaths<Controllable, Event>> Chains
            = new Dictionary<InputMapping, ChainPaths<Controllable, Event>>();

        public static readonly ChainTemplateBuilder DefaultBuilder;
        public static ConfigurableBehaviorFactory<Controllable, Action> Preset(Action defaultAction)
            => new ConfigurableBehaviorFactory<Controllable, Action>(DefaultBuilder, defaultAction);

        static Controllable()
        {
            DefaultBuilder = new ChainTemplateBuilder();

            // set up all chain paths for the input mappings
            // set up all templates
            foreach (InputMapping name in InputMapping.Members)
            {
                DefaultBuilder.AddTemplate<Event>(name);
                Chains[name] = new ChainPaths<Controllable, Event>(name);
            }
        }
    }
}