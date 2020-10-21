using System.Collections.Generic;
using Utils.Vector;

namespace Core.Targeting
{
    public interface ICalculator<T, M>
        where T : Target
    {
        IEnumerable<T> CalculateTargets(
            TargetEvent<T> targetEvent,
            Piece initialPiece,
            Piece rotatedPiece,
            M meta);
    }

    public class MultiCalculator<T, U, M> : ICalculator<T, M>
        where T : Target, ITarget<T, M>, new()
        where U : MultiTarget<T, M>, new()
    {
        public IEnumerable<T> CalculateTargets(
            TargetEvent<T> targetEvent,
            Piece initialPiece,
            Piece rotatedPiece,
            M meta)
        {
            var target = new U
            {
                direction = rotatedPiece.dir,
                initialPiece = initialPiece
            };

            var cell = targetEvent.spot.GetCellRelative(rotatedPiece.pos);
            return target.CalculateTargets(targetEvent, cell, meta);
        }
    }

    public class SimpleCalculator<T, M> : ICalculator<T, M>
        where T : Target, ITarget<T, M>, new()
    {
        public IEnumerable<T> CalculateTargets(
            TargetEvent<T> targetEvent, Piece initialPiece, Piece rotatedPiece, M meta)
        {
            var target = new T
            {
                direction = rotatedPiece.dir,
                initialPiece = initialPiece
            };
            var cell = targetEvent.spot.GetCellRelative(rotatedPiece.pos);
            target.CalculateTargetedEntity(targetEvent, cell, meta);
            yield return target;
        }
    }
}