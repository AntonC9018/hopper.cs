using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Shared.Attributes;

namespace Hopper.TestContent.Freezing
{
    public class FreezingEntityModifier : IEntityModifier
    {
        public Entity outerEntity;

        public FreezingEntityModifier(Entity outerEntity)
        {
            this.outerEntity = outerEntity;
        }
    }

    [InstanceExport]
    public partial class Freeze
    {
        [InstanceExport] [RequiringInit] 
        public static Freeze Default = new Freeze();

        public EntityModifierIndex<FreezingEntityModifier> Index;
        
        
        [Export(Chain = "Ticking.Do", Priority = PriorityRank.High, Dynamic = true)] 
        public void Remove(Entity actor) 
        {
            if (actor.TryGetComponent(Index.ComponentIndex, out var modifier))
            {
                // this also removes this modifier, so we're done
                modifier.outerEntity.Die();
            }
        }

        public Freeze()
        {
            Index = new EntityModifierIndex<FreezingEntityModifier>(InstantiateAndBind, Unbind);
        }

        public void Init()
        {
            Index.Init();
        }

        public FreezingEntityModifier InstantiateAndBind(Entity actor) 
        {
            var transform = actor.GetTransform();
            var iceCube = World.Global.SpawnEntity(
                IceCube.Factory, transform.position, transform.orientation);

            // Setup the Ice Cube
            IceCubeComponent.AddTo(iceCube, actor);

            // Block Attack Dig Move (Or simply acting?)

            return new FreezingEntityModifier(outer);
        }

        public void Unbind(FreezingEntityModifier modifier, Entity actor)
        {
            modifier.outerEntity.Die();
        }
    }
}