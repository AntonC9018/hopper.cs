using Hopper.Core.Chains;
using Hopper.Core;
using Hopper.Test_Content.Utils;
using Hopper.Utils.Chains;

namespace Hopper.Test_Content.Status.Freeze
{
    public class FreezeStatus : Status<FreezeData>
    {
        private EntityFactory<IceCube> m_iceCubeFactory;

        public FreezeStatus(int defaultResValue, EntityFactory<IceCube> iceCubeFactory)
            : base(builder.ToStatic(), FreezeStat.Path, defaultResValue)
        {
            m_iceCubeFactory = iceCubeFactory;
        }

        protected override void Reapply(FreezeData existingData, FreezeData newData)
        {
            // restore its health too
            base.Reapply(existingData, newData);
        }

        protected override void Apply(Entity target, FreezeData statusData)
        {
            base.Apply(target, statusData);

            target.RemoveFromGrid();

            var iceCube = target.World.SpawnHangingEntity(
                m_iceCubeFactory, target.Pos, target.Orientation);
            iceCube.Captured = target;

            iceCube.ResetInGrid();

            statusData.outerEntity = iceCube;
        }

        public override void Remove(Entity entity)
        {
            var outerEntity = m_tinker.GetStore(entity).outerEntity;
            if (outerEntity.IsDead == false)
            {
                outerEntity.Die();
            }
            else
            {
                base.Remove(entity);
            }
        }
        private static ChainDefBuilder builder = new ChainDefBuilder()
            .AddHandler_InsteadOf_Attack_Dig_Move(Handlers.StopPropagate, PriorityRanks.High);
    }
}