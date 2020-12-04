using Chains;
using Core;
using Core.Behaviors;
using Test.Utils;

namespace Test
{
    public class FreezeStatus : Status<FreezeData>
    {
        public FreezeStatus(int defaultResValue)
            : base(builder.ToStatic(), FreezeStat.Path, defaultResValue)
        {
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
                IceCube.Factory, target.Pos, target.Orientation);
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
        public static readonly FreezeStatus Status = new FreezeStatus(1);
    }
}