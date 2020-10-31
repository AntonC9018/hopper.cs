namespace Core
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
        DIRECTIONAL_WALL = 9,
        TICK_PLAYER = 10,
        TICK_REAL = 11
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
            if (Layer.REAL.ToIndex() + 1 != (int)Phase.REAL)
            {
                throw new System.Exception("REAL is not where it should be");
            }
            if (Layer.MISC.ToIndex() + 1 != (int)Phase.MISC)
            {
                throw new System.Exception("MISC is not where it should be");
            }
            if (Layer.WALL.ToIndex() + 1 != (int)Phase.WALL)
            {
                throw new System.Exception("WALL is not where it should be");
            }
            if (Layer.PROJECTILE.ToIndex() + 1 != (int)Phase.PROJECTILE)
            {
                throw new System.Exception("PROJECTILE is not where it should be");
            }
            if (Layer.GOLD.ToIndex() + 1 != (int)Phase.GOLD)
            {
                throw new System.Exception("GOLD is not where it should be");
            }
            if (Layer.FLOOR.ToIndex() + 1 != (int)Phase.FLOOR)
            {
                throw new System.Exception("FLOOR is not where it should be");
            }
            if (Layer.TRAP.ToIndex() + 1 != (int)Phase.TRAP)
            {
                throw new System.Exception("TRAP is not where it should be");
            }
            if (Layer.DROPPED.ToIndex() + 1 != (int)Phase.DROPPED)
            {
                throw new System.Exception("DROPPED is not where it should be");
            }
            if (Layer.DIRECTIONAL_WALL.ToIndex() + 1 != (int)Phase.DIRECTIONAL_WALL)
            {
                throw new System.Exception("DIRECTIONAL_WALL is not where it should be");
            }
        }
    }
}