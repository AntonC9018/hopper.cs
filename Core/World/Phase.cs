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
    }
}