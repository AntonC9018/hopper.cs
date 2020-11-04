using System.Collections.Generic;
using Core.Utils.Vector;

namespace Core.Targeting
{
    public interface ICalculator<T, M>
        where T : Target
    {
        IEnumerable<T> CalculateTargets(
            TargetEvent<T> targetEvent, Piece rotatedPiece, M meta);
    }

    public class MultiCalculator<T, U, M> : ICalculator<T, M>
        where T : Target, ITarget<T, M>, new()
        where U : MultiTarget<T, M>, new()
    {
        private Layer m_skipLayer;
        private Layer m_targetedLayer;

        public MultiCalculator(Layer skipLayer, Layer targetedLayer)
        {
            m_skipLayer = skipLayer;
            m_targetedLayer = targetedLayer;
        }

        public IEnumerable<T> CalculateTargets(
            TargetEvent<T> targetEvent,
            Piece piece,
            M meta)
        {
            var target = new U
            {
                piece = piece
            };

            var cell = targetEvent.spot.GetCellRelative(piece.pos);
            if (cell != null)
            {
                return target.CalculateTargets(targetEvent, cell, meta, m_skipLayer, m_targetedLayer);
            }
            return new T[0];
        }
    }

    public class SimpleCalculator<T, M> : ICalculator<T, M>
        where T : Target, ITarget<T, M>, new()
    {
        private Layer m_skipLayer;
        private Layer m_targetedLayer;

        public SimpleCalculator(Layer skipLayer, Layer targetedLayer)
        {
            m_skipLayer = skipLayer;
            m_targetedLayer = targetedLayer;
        }

        public IEnumerable<T> CalculateTargets(
            TargetEvent<T> targetEvent, Piece piece, M meta)
        {
            var target = new T
            {
                piece = piece,
            };
            var cell = targetEvent.spot.GetCellRelative(piece.pos);
            if (cell != null)
            {
                target.CalculateTargetedEntity(targetEvent, cell, m_skipLayer, m_targetedLayer);
                target.ProcessMeta(meta);
                yield return target;
            }
        }
    }
}