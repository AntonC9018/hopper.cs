using System.Collections.Generic;
using Chains;
using System.Reflection;
using Core.Utils.Vector;
using System.Runtime.Serialization;

namespace Core.Behaviors
{
    public class InputMapping : ChainName
    {
        public readonly static InputMapping Up = new InputMapping("Up");
        public readonly static InputMapping Down = new InputMapping("Down");
        public readonly static InputMapping Left = new InputMapping("Left");
        public readonly static InputMapping Right = new InputMapping("Right");
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
    public class Controllable : Behavior
    {

        public class Event : StandartEvent
        {
            public void SetAction(Action action, IntVector2 dir)
            {
                this.action = action.Copy();
                this.action.direction = dir;
            }

            public void SetAction(Action action)
            {
                var prev = this.action;
                this.action = action.Copy();
                if (prev != null)
                {
                    this.action.direction = prev.direction;
                }
            }
        }

        public class Config : BehaviorConfig
        {
            public Action defaultAction;
        }

        public Action config_defaultAction;

        public override void Init(Entity entity, BehaviorConfig _config)
        {
            var config = (Config)_config;
            config_defaultAction = config.defaultAction;
            base.Init(entity, null);
        }

        public Action ConvertInputToAction(InputMapping input)
        {
            var ev = new Event { actor = m_entity };
            GetChain<Event>(input).Pass(ev);
            return ev.action;
        }

        static System.Action<Event> Default(IntVector2 dir)
        {
            return ev => ev.SetAction(ev.actor.Behaviors.Get<Controllable>().config_defaultAction, dir);
        }

        public static readonly Dictionary<InputMapping, ChainPaths<Controllable, Event>> Chains
            = new Dictionary<InputMapping, ChainPaths<Controllable, Event>>();

        static Controllable()
        {
            var builder = new ChainTemplateBuilder();

            // set up all chain paths for the input mappings
            // set up all templates
            foreach (InputMapping name in InputMapping.Members)
            {
                builder.AddTemplate<Event>(name);
                Chains[name] = new ChainPaths<Controllable, Event>(name);
            }

            builder
                .GetTemplate<Event>(InputMapping.Up)
                .AddHandler(Default(IntVector2.Up), PriorityRanks.Highest)
                .End()

                .GetTemplate<Event>(InputMapping.Right)
                .AddHandler(Default(IntVector2.Right), PriorityRanks.Highest)
                .End()

                .GetTemplate<Event>(InputMapping.Down)
                .AddHandler(Default(IntVector2.Down), PriorityRanks.Highest)
                .End()

                .GetTemplate<Event>(InputMapping.Left)
                .AddHandler(Default(IntVector2.Left), PriorityRanks.Highest)
                .End();

            BehaviorFactory<Controllable>.s_builder = builder;
        }
    }
}