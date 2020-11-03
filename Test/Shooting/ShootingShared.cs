using System.Collections.Generic;
using Core;
using Core.Behaviors;
using Core.History;
using Core.Stats.Basic;
using Core.Targeting;
using Core.Utils.Vector;

namespace Test
{
    public class ShootingShared
    {
        public static readonly UpdateCode UpdateCode = new UpdateCode("shooting");

        private Layer m_targetedLayer;
        private Layer m_skipLayer;
        protected bool m_stopAfterFirstAttack;

        protected ShootingShared(Layer targetedLayer, Layer skipLayer, bool stopAfterFirstAttack)
        {
            m_targetedLayer = targetedLayer;
            m_skipLayer = skipLayer;
            m_stopAfterFirstAttack = stopAfterFirstAttack;
        }

        protected void ShootingPrelude(Entity entity, Action action)
        {
            entity.Reorient(action.direction);
            entity.History.Add(entity, UpdateCode);
        }

        protected ShootingInfo GetInitialShootInfo(IWorldSpot spot, IntVector2 direction)
        {
            IntVector2 currentOffsetVec = direction;

            var info = new ShootingInfo();

            while (true)
            {
                var cell = spot.GetCellRelative(currentOffsetVec);

                // off the world or a block is in the way
                if (cell == null || cell.HasBlock(direction, m_skipLayer))
                {
                    info.last_checked_pos = spot.Pos + currentOffsetVec;
                    return info;
                }

                var target = cell.GetEntityFromLayer(direction, m_targetedLayer);
                if (target != null)
                {
                    info.attacked_targets.Add(target);
                    if (m_stopAfterFirstAttack)
                    {
                        info.last_checked_pos = spot.Pos + currentOffsetVec;
                        return info;
                    }
                }

                currentOffsetVec += direction;
            }
        }

        protected bool ShootAtAnon(Entity attacked, IntVector2 direction, Attack attack)
        {
            return attacked.Behaviors.Get<Attackable>()
                .Activate(direction, new Attackable.Params(attack, null));
        }

        protected void PushBack(Entity attacked, IntVector2 direction, Push push)
        {
            attacked.Behaviors.Get<Pushable>().Activate(direction, push);
        }
    }
}