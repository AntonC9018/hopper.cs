using Hopper.Core;
using Hopper.Core.Behaviors;
using Chains;

namespace Hopper.Test_Content
{
    public interface ICapture
    {
        Entity Captured { get; set; }
    }

    public class IceCube : Entity, ICapture
    {
        public Entity Captured { get; set; }

        public static Retoucher MoveCapturedRetoucher = new Retoucher(
            new TemplateChainDefBuilder()
                .AddDef(Displaceable.Do)
                .AddHandler(DisplaceCaptured)
                .End().ToStatic()
        );

        public static EntityFactory<IceCube> Factory = CreateFactory();
        public static EntityFactory<IceCube> CreateFactory()
        {
            return new EntityFactory<IceCube>()
                .AddBehavior<Displaceable>()
                .AddBehavior<Attackable>()
                .AddBehavior<Pushable>()
                .AddBehavior<Damageable>()
                .Retouch(MoveCapturedRetoucher)
                .AddDieListener(ReleaseOnDeath);
        }

        private static void DisplaceCaptured(ActorEvent ev)
        {
            var icapture = (ICapture)ev.actor;
            icapture.Captured.Pos = ev.actor.Pos;
        }

        private static void ReleaseOnDeath(IceCube iceCube)
        {
            // release
            iceCube.Captured.ResetInGrid();
            // remove the status effect
            FreezeStatus.Status.Remove(iceCube.Captured);
            // TODO: apply 1 invulnerable to the captured entity
        }
    }
}