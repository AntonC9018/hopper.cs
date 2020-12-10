using Hopper.Utils.Vector;

namespace Hopper.Core.Targeting
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
    }
}