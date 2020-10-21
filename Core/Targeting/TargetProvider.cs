using System.Collections.Generic;
using Utils.Vector;
using Chains;
using Utils;

namespace Core.Targeting
{

    public class TargetEvent<T> : CommonEvent where T : Target
    {
        public List<T> targets;
    }

    public class TargetProvider<T> : IProvideTargets<T> where T : Target, new()
    {
        private Pattern m_pattern;
        private Chain<TargetEvent<T>> m_chain;
        private System.Func<TargetEvent<T>, bool> m_stopFunc;
        private ICalculator<T> m_calculator;

        public TargetProvider(
            Pattern pattern,
            Chain<TargetEvent<T>> chain,
            System.Func<TargetEvent<T>, bool> stopFunc,
            ICalculator<T> calculator)
        {
            this.m_pattern = pattern;
            this.m_chain = chain;
            this.m_stopFunc = stopFunc;
            this.m_calculator = calculator;
        }

        public IEnumerable<T> GetParticularTargets(CommonEvent commonEvent)
        {
            var targets = new List<T>();
            double angle = IntVector2.Right.AngleTo(commonEvent.action.direction);

            for (int i = 0; i < this.m_pattern.pieces.Count; i++)
            {
                var piece = this.m_pattern.pieces[i].Rotate(angle);

                targets.AddRange(
                    m_calculator.CalculateTargets(
                        commonEvent, this.m_pattern.pieces[i], piece
                    )
                );
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

        public List<Target> GetTargets(CommonEvent commonEvent)
        {
            return GetParticularTargets(commonEvent).ConvertAllToList(t => (Target)t);
        }
    }
}