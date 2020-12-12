using System.Collections.Generic;
using Hopper.Utils.Chains;
using Hopper.Core.Behaviors.Basic;
using Hopper.Core.Stats.Basic;
using Hopper.Utils.Vector;

namespace Hopper.Core.Targeting
{
    public class BufferedAtkTargetProvider : IBufferedAtkTargetProvider
    {
        private IPattern m_pattern;
        private Chain<TargetEvent<AtkTarget>> m_chain;
        private System.Func<TargetEvent<AtkTarget>, bool> m_stopFunc;
        private Layer m_skipLayer;
        private Layer m_targetLayer;

        public BufferedAtkTargetProvider(
            IPattern pattern,
            Chain<TargetEvent<AtkTarget>> chain,
            System.Func<TargetEvent<AtkTarget>, bool> stopFunc,
            Layer skipLayer,
            Layer targetLayer)
        {
            m_pattern = pattern;
            m_chain = chain;
            m_stopFunc = stopFunc;
            m_skipLayer = skipLayer;
            m_targetLayer = targetLayer;
        }

        public List<AtkTarget> GetTargets(IWorldSpot spot, IntVector2 direction, Attack attack)
        {
            var targetEvent = new TargetEvent<AtkTarget>
            {
                spot = spot,
                direction = direction,
                targets = new List<AtkTarget>()
            };

            foreach (var rotatedPiece in m_pattern.GetPieces(spot, direction))
            {
                Cell cell = spot.GetCellRelative(rotatedPiece.pos);
                if (cell != null && cell.HasBlock(direction, m_skipLayer) == false)
                {
                    var entity = cell.GetEntityFromLayer(direction, m_targetLayer);
                    if (entity != null)
                    {
                        var atkness = entity.Behaviors.Has<Attackable>()
                            ? entity.Behaviors.Get<Attackable>().GetAtkCondition(attack)
                            : Attackness.NEVER;
                        var target = new AtkTarget(atkness, rotatedPiece, entity);
                        targetEvent.targets.Add(target);
                    }
                }
            }

            m_chain.Pass(targetEvent, m_stopFunc);

            return targetEvent.targets;
        }
    }
}