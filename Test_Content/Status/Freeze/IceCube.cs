using Hopper.Core;
using Hopper.Core.Behaviors.Basic;
using Hopper.Core.Chains;

namespace Hopper.Test_Content.Status.Freezing
{
    public interface ICapture
    {
        Entity Captured { get; set; }
    }

    public class IceCube : Entity, ICapture
    {
        public Entity Captured { get; set; }

        public static readonly Retoucher MoveCapturedRetoucher = new Retoucher(
            new TemplateChainDefBuilder()
                .AddDef(Displaceable.Do)
                .AddHandler(DisplaceCaptured)
                .End().ToStatic()
        );
        public static readonly EntityFactory<IceCube> Factory = CreateFactory();

        public static EntityFactory<IceCube> CreateFactory()
        {
            return new EntityFactory<IceCube>()
                .AddBehavior(Displaceable.DefaultPreset)
                .AddBehavior(Attackable.DefaultPreset)
                .AddBehavior(Pushable.Preset)
                .AddBehavior(Damageable.Preset)
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
            Freeze.Status.Remove(iceCube.Captured);
            // TODO: apply 1 invulnerable to the captured entity
        }
    }
}