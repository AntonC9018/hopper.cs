using System.Collections.Generic;
using Utils.Vector;

namespace Core.Targeting
{
    public interface ICalculator<out T> where T : Target
    {
        IEnumerable<T> CalculateTargets(
            CommonEvent commonEvent,
            Piece initialPiece,
            Piece rotatedPiece);
    }

    public class MultiCalculator<T> : ICalculator<T> where T : Target, new()
    {
        public IEnumerable<T> CalculateTargets(
            CommonEvent commonEvent,
            Piece initialPiece,
            Piece rotatedPiece)
        {
            var target = new MultiTarget<T>
            {
                direction = rotatedPiece.dir,
                initialPiece = initialPiece
            };

            var cell = commonEvent.actor.GetCellRelative(rotatedPiece.pos);
            return target.CalculateTargets(commonEvent, cell);
        }
    }

    public class SimpleCalculator<T> : ICalculator<T> where T : Target, new()
    {
        public IEnumerable<T> CalculateTargets(
            CommonEvent commonEvent,
            Piece initialPiece,
            Piece rotatedPiece)
        {
            var target = new T
            {
                direction = rotatedPiece.dir,
                initialPiece = initialPiece
            };
            var cell = commonEvent.actor.GetCellRelative(rotatedPiece.pos);
            target.CalculateTargetedEntity(commonEvent, cell);
            yield return target;
        }
    }
}