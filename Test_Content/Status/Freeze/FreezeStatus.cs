using Hopper.Core.Chains;
using Hopper.Core;
using Hopper.Test_Content.Utils;
using Hopper.Utils.Chains;

namespace Hopper.Test_Content.Status.Freezing
{
    public class FreezeStatus : Status<FreezeData>
    {
        private static readonly ChainDefBuilder builder;

        public FreezeStatus(int defaultResValue)
            : base(builder.ToStatic(), Freeze.Path, defaultResValue)
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

        static FreezeStatus()
        {
            builder = new ChainDefBuilder()
                .AddHandler_InsteadOf_Attack_Dig_Move(Handlers.StopPropagate, PriorityRank.High);
        }
    }
}