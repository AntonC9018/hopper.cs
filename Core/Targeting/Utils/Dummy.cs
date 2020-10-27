using Core.Utils.Vector;

namespace Core.Targeting
{
    public class Dummy : IWorldSpot
    {
        public Dummy(IntVector2 pos, World world)
        {
            Pos = pos;
            World = world;
        }

        public IntVector2 Pos { get; private set; }
        public World World { get; private set; }

        public Cell GetCellRelative(IntVector2 dir)
        {
            return World.m_grid.GetCellAt(Pos + dir);
        }
    }
}