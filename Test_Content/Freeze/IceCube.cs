using Hopper.Core;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Shared.Attributes;

namespace Hopper.TestContent.Freezing
{
    public class IceCubeComponent : IComponent
    {
        public Entity captured;
    }

    [EntityType]
    public static class IceCube
    {
        public static EntityFactory Factory;

        public static void AddComponents(Entity subject)
        {
            SimplePassiveRealBase.AddComponents(subject);
        }

        public static void InitComponents(Entity subject)
        {
            SimplePassiveRealBase.InitComponents(subject);
        }

        public static void Retouch(Entity subject)
        {
        }

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

        [Export(Chain = "Displaceable.Do", Dynamic = true)]
        public static void DisplaceCaptured(IceCubeComponent iceCubeComponent, Transform transform)
        {
            iceCubeComponent.captured.GetTransform().position = transform.position;
        }

        [Export(Chain = "Damageable.Death", Dynamic = true)]
        private static void ReleaseOnDeath(IceCubeComponent iceCubeComponent)
        {
            // release
            iceCubeComponent.captured.GetTransform().ResetInGrid();
            // remove the status effect
            Freeze.Default.Index.RemoveFrom(iceCubeComponent.captured);
            // TODO: apply 1 invulnerable to the captured entity
        }
    }
}