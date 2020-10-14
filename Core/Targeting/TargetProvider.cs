using System.Collections.Generic;
using Utils.Vector;
using Chains;
using Core.Behaviors;

namespace Core.Targeting
{
    public class TargetEvent<T> : CommonEvent where T : Target
    {
        public List<T> targets;
    }

    public class TargetProvider<T> where T : Target, new()
    {
        private List<Piece> m_pattern;
        private Chain<TargetEvent<T>> m_chain;
        private System.Func<TargetEvent<T>, bool> m_stopFunc;
        private Layer m_targetedLayer;

        public TargetProvider(
            List<Piece> pattern,
            Chain<TargetEvent<T>> chain,
            System.Func<TargetEvent<T>, bool> stopFunc,
            Layer targetedLayer = Layer.REAL | Layer.MISC | Layer.WALL)
        {
            this.m_pattern = pattern;
            this.m_chain = chain;
            this.m_stopFunc = stopFunc;
            this.m_targetedLayer = targetedLayer;
        }

        public List<T> GetTargets(CommonEvent commonEvent)
        {
            var targets = new List<T>();
            double angle = IntVector2.Right.AngleTo(commonEvent.action.direction);

            for (int i = 0; i < this.m_pattern.Count; i++)
            {
                var piece = this.m_pattern[i].Rotate(angle);


                var target = new T
                {
                    direction = piece.dir,
                    index = i,
                    initialPiece = this.m_pattern[i]
                };

                target.CalculateTargets(commonEvent.actor.GetCellRelative(piece.pos), m_targetedLayer);
                target.CalculateCondition(commonEvent);
                targets.Add(target);
            }

            var ev = new TargetEvent<T>
            {
                targets = targets,
                actor = commonEvent.actor,
                action = commonEvent.action
            };

            m_chain.Pass(ev, m_stopFunc);

            return ev.targets;
        }
    }
}