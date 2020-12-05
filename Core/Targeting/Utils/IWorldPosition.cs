using Hopper.Utils.Vector;

namespace Hopper.Core.Targeting
{
    public interface IWorldSpot
    {
        IntVector2 Pos { get; }
        World World { get; }
        Cell GetCellRelative(IntVector2 dir);
    }
}