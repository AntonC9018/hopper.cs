using Hopper.Utils.Vector;

namespace Hopper.Core
{
    public interface IWorldSpot
    {
        IntVector2 Pos { get; }
        World World { get; }
    }

    public static class IWorldSpotExtensions
    {
        public static Cell GetCellRelative(this IWorldSpot spot, IntVector2 dir)
        {
            return spot.World.grid.GetCellAt(spot.Pos + dir);
        }

        public static IntVector2 GetPosRelative(this IWorldSpot spot, IntVector2 offset)
        {
            return spot.Pos + offset;
        }

        public static bool HasBlockRelative(this IWorldSpot spot, IntVector2 offset)
        {
            return HasBlockRelative(spot, offset, ExtendedLayer.BLOCK);
        }

        public static bool HasBlockRelative(this IWorldSpot spot, IntVector2 offset, Layer layer)
        {
            var cell = spot.GetCellRelative(offset);
            if (cell == null) return true;
            return cell.HasBlock(offset.Sign(), layer);
        }

        public static Cell GetCell(this IWorldSpot spot) => spot.World.grid.GetCellAt(spot.Pos);
    }
}