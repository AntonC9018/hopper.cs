using Hopper.Core;
using Hopper.Core.History;
using Hopper.Core.Stats.Basic;
using Hopper.Core.Targeting;
using Hopper.Utils.Vector;

namespace Hopper.Test_Content
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

        public static void ShootingPrelude(Entity entity, Action action)
        {
            entity.Reorient_(action.direction);
            entity.History.Add(entity, UpdateCode);
        }
        /*
            So the targets provider became really messy once I tried to generalize it
            The algorithm just isn't suited for some types of problems
            For example, here we would:
                    1. generate the targets on the fly, which is messy in the target provider
                    2. stop generating the targets, once a block is met, which is impossible
                    with the target provider, since it does two passes: one for getting
                    all the possile targets and the other one for cleaning them up.
                    3. get info on the positions checked, which is again impossible with
                    the target provider, since it removes the targets which didn't target
                    an entity and if you make it to not do that, then what is the point
                    of `target` provider?
            I can't come up with a good abstraction for this yet, so I'm going to implement it
            separately for shooting here and we'll see later. Implementing inefficient hacks
            to the current target provider algorithm for first makes me feel bad and for second
            overcomplicates an already bloated piece of code.

            Update: deleted the overgeneralized prior code, leaving only useful interfaces.
            Event still, if we wish to generate targets dynamically, that is, when the pattern
            generates points on the fly, it is easier to define some logic like below, than 
            to elaborate a more generic way of generating this. Although I can see possible room
            for thought in `dynamic patterns`, let's call them so, as well. The key would probably
            be to return some more powerful info than just the piece, like the piece of cell
            and whether we should continue iterating or return from the function.
        */
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
    }
}