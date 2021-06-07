using Hopper.Core;
using Hopper.Core.Components;
using Hopper.Shared.Attributes;
using Hopper.TestContent.SimpleMobs;
using Hopper.Core.Components.Basic;
using Hopper.Core.WorldNS;

namespace Hopper.TestContent.FreezingNS
{
    public partial class IceCubeComponent : IComponent
    {
        [Inject] public Entity captured;
    }

    [EntityType]
    public static partial class IceCube
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

        [Export(Chain = "Displaceable.After", Dynamic = true)]
        public static void DisplaceCaptured(IceCubeComponent iceCubeComponent, Transform transform)
        {
            iceCubeComponent.captured.GetTransform().position = transform.position;
        }

        [Export(Chain = "+Entity.Death", Dynamic = true)]
        private static void ReleaseOnDeath(IceCubeComponent iceCubeComponent)
        {
            // release
            // remove the status effect
            iceCubeComponent.captured.GetFreezingEntityModifier()
                .RemoveLogic(iceCubeComponent.captured);
            // TODO: apply 1 invulnerable to the captured entity
        }
    }
}