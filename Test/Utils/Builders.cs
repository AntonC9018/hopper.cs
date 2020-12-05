using Chains;
using Hopper.Core;
using Hopper.Core.Behaviors;
using Hopper.Core.Utils.Vector;

namespace Test
{
    public static class Builders
    {
        public static ChainDefBuilder AddHandler_InsteadOf_Attack_Dig_Move(
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

                .AddDef(Controllable.Chains[InputMapping.Vector])
                .AddHandler(handler, priority)

                .End();
        }
    }
}