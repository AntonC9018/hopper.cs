using Hopper.Core;
using Hopper.Core.Components.Basic;

namespace Hopper.TestContent.Floor
{
    public class IceFloor : Entity
    {
        public override Layer Layer => Layer.FLOOR;
        public static EntityFactory<IceFloor> Factory = CreateFactory(Slide.Status);

        public static EntityFactory<IceFloor> CreateFactory(SlideStatus slideStatus)
        {
            return new EntityFactory<IceFloor>()
                .AddBehavior(Attackable.DefaultPreset) // bombs can destroy it
                .AddBehavior(Sliding.Preset(slideStatus));
        }

        public void Melt()
        {
            Die();
            var water = World.SpawnEntity(Water.Factory, m_pos);
            water.Behaviors.Get<Acting>().CalculateNextAction();
            water.Behaviors.Get<Acting>().Activate();
        }
    }
}