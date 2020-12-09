using Hopper.Utils;

namespace Hopper.Core
{
    public enum Phase
    {
        PLAYER = 0,
        REAL = 1,
        MISC = 2,
        WALL = 3,
        PROJECTILE = 4,
        GOLD = 5,
        FLOOR = 6,
        TRAP = 7,
        DROPPED = 8,
        TICK_PLAYER = 9,
        TICK_REAL = 10
    }

    public static class PhaseLayerExtensions
    {
        public static Phase ToPhase(this Layer layer)
        {
            if ((int)layer == 0)
            {
                return Phase.PLAYER;
            }
            return (Phase)layer.ToIndex();
        }

        public static void ThrowIfPhasesAreWrong()
        {
            Assert.That(Layer.REAL.ToIndex() + 1 == (int)Phase.REAL, "REAL is not where it should be");
            Assert.That(Layer.MISC.ToIndex() + 1 == (int)Phase.MISC, "MISC is not where it should be");
            Assert.That(Layer.WALL.ToIndex() + 1 == (int)Phase.WALL, "WALL is not where it should be");
            Assert.That(Layer.PROJECTILE.ToIndex() + 1 == (int)Phase.PROJECTILE, "PROJECTILE is not where it should be");
            Assert.That(Layer.GOLD.ToIndex() + 1 == (int)Phase.GOLD, "GOLD is not where it should be");
            Assert.That(Layer.FLOOR.ToIndex() + 1 == (int)Phase.FLOOR, "FLOOR is not where it should be");
            Assert.That(Layer.TRAP.ToIndex() + 1 == (int)Phase.TRAP, "TRAP is not where it should be");
            Assert.That(Layer.DROPPED.ToIndex() + 1 == (int)Phase.DROPPED, "DROPPED is not where it should be");
        }
    }
}