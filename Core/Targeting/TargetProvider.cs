using System.Collections.Generic;
using Core.Utils.Vector;
using Chains;
using Core.Utils;

namespace Core.Targeting
{
    public class TargetProvider<T, M> : IProvideTargets<T, M>
        where T : Target, new()
    {
        private IPattern m_pattern;
        private Chain<TargetEvent<T>> m_chain;
        private System.Func<TargetEvent<T>, bool> m_stopFunc;
        private ICalculator<T, M> m_calculator;

        public TargetProvider(
            IPattern pattern,
            Chain<TargetEvent<T>> chain,
            System.Func<TargetEvent<T>, bool> stopFunc,
            ICalculator<T, M> calculator)
        {
            this.m_pattern = pattern;
            this.m_chain = chain;
            this.m_stopFunc = stopFunc;
            this.m_calculator = calculator;
        }

        public IEnumerable<T> GetParticularTargets(TargetEvent<T> targetEvent, M meta)
        {
            var targets = new List<T>();

            foreach (var rotatedPiece in m_pattern.GetPieces(targetEvent.spot, targetEvent.dir))
            {
                var calculatedTargets = m_calculator.CalculateTargets(
                    targetEvent, rotatedPiece, meta);

                targets.AddRange(calculatedTargets);
            }

            targetEvent.targets = targets;
            m_chain.Pass(targetEvent, m_stopFunc);

            return targetEvent.targets;
        }

        public List<Target> GetTargets(TargetEvent<T> targetEvent, M meta)
        {
            return GetParticularTargets(targetEvent, meta).ConvertAllToList(t => (Target)t);
        }
    }
}