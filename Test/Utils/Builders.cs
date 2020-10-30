using Chains;
using Core;
using Core.Behaviors;
using Core.Utils.Vector;

namespace Test
{
    public static class Builders
    {
        // public static ChainDefBuilder Create_HandlerInsteadOf_Attack_Dig_Move(
        //     System.Action<ActorEvent> handler, PriorityRanks priority = PriorityRanks.Medium)
        // {
        //     var builder = new ChainDefBuilder();
        //     HandlerInsteadOf_Attack_Dig_Move(builder);
        // }

        public static ChainDefBuilder Add_HandlerInsteadOf_Attack_Dig_Move(
            this ChainDefBuilder builder,
            System.Action<EventBase> handler,
            PriorityRanks priority = PriorityRanks.Default)
        {
            return builder
                .AddDef(Attacking.Do)
                .AddHandler(handler, priority)

                .AddDef(Digging.Do)
                .AddHandler(handler, priority)

                .AddDef(Moving.Do)
                .AddHandler(handler, priority)
                .End();
        }

        public static ChainDefBuilder AddHandler_ToAllVectorInputs(
            this ChainDefBuilder builder,
            System.Action<Controllable.Event> handler,
            PriorityRanks priority = PriorityRanks.Default)
        {
            return builder

                .AddDef(Controllable.Chains[InputMapping.Up])
                .AddHandler(handler, priority)

                .AddDef(Controllable.Chains[InputMapping.Down])
                .AddHandler(handler, priority)

                .AddDef(Controllable.Chains[InputMapping.Left])
                .AddHandler(handler, priority)

                .AddDef(Controllable.Chains[InputMapping.Right])
                .AddHandler(handler, priority)

                .End();
        }

        // public static ChainDefBuilder Add_HandlerWithDirection_ToAllVectorInputs(
        //     this ChainDefBuilder builder,
        //     System.Func<IntVector2, System.Action<Controllable.Event>> handlerGenerator,
        //     PriorityRanks priority = PriorityRanks.Default)
        // {
        //     return builder
        //         .AddDef(Controllable.Chains[InputMapping.])
        //         .AddHandler(handler, priority)

        //         .AddDef(Digging.Do)
        //         .AddHandler(handler, priority)

        //         .AddDef(Moving.Do)
        //         .AddHandler(handler, priority)
        //         .End();
        // }
    }
}