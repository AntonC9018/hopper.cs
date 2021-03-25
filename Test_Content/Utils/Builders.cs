using Hopper.Core.Chains;
using Hopper.Utils.Chains;
using Hopper.Core.Components.Basic;

namespace Hopper.Test_Content
{
    public static class Builders
    {
        public static ChainDefBuilder AddHandler_InsteadOf_Attack_Dig_Move(
            this ChainDefBuilder builder,
            System.Action<EventBase> handler,
            PriorityRank priority = PriorityRank.Default)
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
            PriorityRank priority = PriorityRank.Default)
        {
            return builder

                .AddDef(Controllable.Chains[InputMapping.Vector])
                .AddHandler(handler, priority)

                .End();
        }
    }
}