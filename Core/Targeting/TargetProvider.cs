using System.Collections.Generic;
using Utils.Vector;
using Chains;
using Utils;

namespace Core.Targeting
{
    public interface IWorldPosition
    {
        IntVector2 Pos { get; }
        World World { get; }
        Cell GetCellRelative(IntVector2 dir);
    }

    public class TargetEvent<T> : EventBase where T : Target
    {
        public List<T> targets;
        public IntVector2 dir;
        public IWorldPosition spot;

        public TargetEvent(CommonEvent ev)
        {
            dir = ev.action.direction;
            spot = ev.actor;
        }

        public TargetEvent(IntVector2 dir, IWorldPosition worldPosition)
        {
            this.dir = dir;
            this.spot = worldPosition;
        }

        public TargetEvent() { }
    }

    public class TargetProvider<T, E> : IProvideTargets<T, E>
        where T : Target, new()
        where E : TargetEvent<T>
    {
        private Pattern m_pattern;
        private Chain<E> m_chain;
        private System.Func<E, bool> m_stopFunc;
        private ICalculator<T, E> m_calculator;

        public TargetProvider(
            Pattern pattern,
            Chain<E> chain,
            System.Func<E, bool> stopFunc,
            ICalculator<T, E> calculator)
        {
            this.m_pattern = pattern;
            this.m_chain = chain;
            this.m_stopFunc = stopFunc;
            this.m_calculator = calculator;
        }

        public IEnumerable<T> GetParticularTargets(E targetEvent)
        {
            var targets = new List<T>();
            double angle = IntVector2.Right.AngleTo(targetEvent.dir);

            for (int i = 0; i < this.m_pattern.pieces.Count; i++)
            {
                var piece = this.m_pattern.pieces[i].Rotate(angle);

                targets.AddRange(
                    m_calculator.CalculateTargets(
                        targetEvent, this.m_pattern.pieces[i], piece
                    )
                );
            }

            targetEvent.targets = targets;
            m_chain.Pass(targetEvent, m_stopFunc);
            return targetEvent.targets;
        }

        public List<Target> GetTargets(E targetEvent)
        {
            return GetParticularTargets(targetEvent).ConvertAllToList(t => (Target)t);
        }
    }
}