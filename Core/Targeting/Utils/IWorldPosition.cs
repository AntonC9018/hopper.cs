using Utils.Vector;

namespace Core.Targeting
{
    public interface IWorldPosition
    {
        IntVector2 Pos { get; }
        World World { get; }
        Cell GetCellRelative(IntVector2 dir);
    }
}