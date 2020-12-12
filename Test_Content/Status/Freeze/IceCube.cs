using Hopper.Core;
using Hopper.Core.Behaviors.Basic;
using Hopper.Core.Chains;

namespace Hopper.Test_Content.Status.Freeze
{
    public interface ICapture
    {
        Entity Captured { get; set; }
    }

    public class IceCube : Entity, ICapture
    {
        public Entity Captured { get; set; }

        public static Retoucher CreateMoveCapturedRetoucher() => new Retoucher(
            new TemplateChainDefBuilder()
                .AddDef(Displaceable.Do)
                .AddHandler(DisplaceCaptured)
                .End().ToStatic()
        );

        public static EntityFactory<IceCube> CreateFactory(
            Retoucher MoveCapturedRetoucher, FreezeStatus status)
        {
            return new EntityFactory<IceCube>()
                .AddBehavior<Displaceable>()
                .AddBehavior<Attackable>()
                .AddBehavior<Pushable>()
                .AddBehavior<Damageable>()
                .Retouch(MoveCapturedRetoucher)
                .AddDieListener(ReleaseOnDeath(status));
        }

        private static void DisplaceCaptured(ActorEvent ev)
        {
            var icapture = (ICapture)ev.actor;
            icapture.Captured.Pos = ev.actor.Pos;
        }

        private static System.Action<IceCube> ReleaseOnDeath(FreezeStatus status)
        {
            return iceCube =>
            {
                // release
                iceCube.Captured.ResetInGrid();
                // remove the status effect
                status.Remove(iceCube.Captured);
                // TODO: apply 1 invulnerable to the captured entity
            };
        }
    }
}