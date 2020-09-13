using System.Collections.Generic;
using Core.FS;
using Chains;
using System.Reflection;
using Vector;

namespace Core.Behaviors
{
    public static class InputMappings
    {
        public readonly static ChainName Up = ChainName.NextNew();
        public readonly static ChainName Down = ChainName.NextNew();
        public readonly static ChainName Left = ChainName.NextNew();
        public readonly static ChainName Right = ChainName.NextNew();
        public readonly static ChainName Action_0 = ChainName.NextNew();
        public readonly static ChainName Action_1 = ChainName.NextNew();
        public readonly static ChainName Special_0 = ChainName.NextNew();
        public readonly static ChainName Special_1 = ChainName.NextNew();
        public readonly static ChainName Item_0 = ChainName.NextNew();
        public readonly static ChainName Item_1 = ChainName.NextNew();
        public readonly static ChainName Weapon_0 = ChainName.NextNew();
        public readonly static ChainName Weapon_1 = ChainName.NextNew();

        public static IEnumerable<ChainName> Members
        {
            get
            {
                var type = typeof(InputMappings);
                var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);

                foreach (var field in fields)
                    yield return (ChainName)field.GetValue(null);
            }
        }
    }

    public class Input : Behavior
    {

        public class Event : CommonEvent
        {
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

        public Action ConvertInputToAction(ChainName chainName)
        {
            var ev = new Event { actor = m_entity };
            GetChain<Event>(chainName).Pass(ev);
            return ev.action;
        }

        static System.Action<Event> Default(IntVector2 dir)
        {
            return ev =>
            {
                ev.action = ev.actor.GetBehavior<Input>().config_defaultAction.Copy();
                ev.action.direction = dir;
            };
        }

        public static readonly Dictionary<ChainName, ChainPaths<Input, Event>> Chains
            = new Dictionary<ChainName, ChainPaths<Input, Event>>();

        static Input()
        {
            var builder = new ChainTemplateBuilder();

            // set up all chain paths for the input mappings
            // set up all templates
            foreach (ChainName name in InputMappings.Members)
            {
                builder.AddTemplate<Event>(name);
                Chains[name] = new ChainPaths<Input, Event>(name);
            }

            builder
                .GetTemplate<Event>(InputMappings.Up)
                .AddHandler(Default(IntVector2.Up), PriorityRanks.Lowest);

            builder
                .GetTemplate<Event>(InputMappings.Right)
                .AddHandler(Default(IntVector2.Right), PriorityRanks.Lowest);

            builder
                .GetTemplate<Event>(InputMappings.Down)
                .AddHandler(Default(IntVector2.Down), PriorityRanks.Lowest);

            builder
                .GetTemplate<Event>(InputMappings.Left)
                .AddHandler(Default(IntVector2.Left), PriorityRanks.Lowest);

            BehaviorFactory<Input>.s_builder = builder;
        }
    }
}