using Hopper.Core;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat;
using Hopper.Core.Stat.Basic;
using Hopper.Core.Targeting;
using Hopper.Shared.Attributes;

namespace Hopper.Test_Content.Freezing
{
    public class IceCubeComponent : IComponent
    {
        public Entity captured;
    }

    [EntityType(Abstract = true)]
    public static class SimplePassiveRealBase
    {
        public static EntityFactory Factory;

        public static void AddComponents(Entity subject)
        {
            Faction.AddTo(subject, Faction.Flags.Environment);
            Displaceable.AddTo(subject, ExtendedLayer.BLOCK);
            Attackable.AddTo(subject, Attackness.ALWAYS);
            // TODO: Be able to manipulate stats in a simple way.
            Stats.AddTo(subject, Registry.Global._defaultStats);
            Pushable.AddTo(subject);
            Damageable.AddTo(subject, new Health(1));
            Transform.AddTo(subject, Layer.REAL);
        }

        public static void InitComponents(Entity subject)
        {
            subject.GetDisplaceable().DefaultPreset();
            subject.GetAttackable().DefaultPreset();
            subject.GetPushable().DefaultPreset();
            subject.GetDamageable().DefaultPreset();
        }
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