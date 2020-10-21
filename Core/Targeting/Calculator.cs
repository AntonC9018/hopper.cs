using System.Collections.Generic;
using Utils.Vector;

namespace Core.Targeting
{
    public interface ICalculator<out T, in Event>
        where T : Target
        where Event : TargetEvent<T>
    {
        IEnumerable<T> CalculateTargets(
            Event targetEvent,
            Piece initialPiece,
            Piece rotatedPiece);
    }

    public class MultiCalculator<T, U, E> : ICalculator<T, E>
        where T : Target, new()
        where U : MultiTarget<T, E>, new()
        where E : TargetEvent<T>
    {
        public IEnumerable<T> CalculateTargets(
            E targetEvent,
            Piece initialPiece,
            Piece rotatedPiece)
        {
            var target = new U
            {
                direction = rotatedPiece.dir,
                initialPiece = initialPiece
            };

            var cell = targetEvent.spot.GetCellRelative(rotatedPiece.pos);
            return target.CalculateTargets(targetEvent, cell);
        }
    }

    public class SimpleCalculator<T, E> : ICalculator<T, E>
        where T : Target, ITarget<T, E>, new()
        where E : TargetEvent<T>
    {
        public IEnumerable<T> CalculateTargets(
            E targetEvent,
            Piece initialPiece,
            Piece rotatedPiece)
        {
            var target = new T
            {
                direction = rotatedPiece.dir,
                initialPiece = initialPiece
            };
            var cell = targetEvent.spot.GetCellRelative(rotatedPiece.pos);
            target.CalculateTargetedEntity(targetEvent, cell);
            yield return target;
        }
    }
}