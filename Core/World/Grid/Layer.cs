using Hopper.Shared.Attributes;

namespace Hopper.Core.WorldNS
{

    // This indicates the order in which actions are executed
    [Flags] public enum Layers
    {
        REAL       = 1,
        MISC       = 2,
        WALL       = 4,
        PROJECTILE = 8,
        ITEM       = 16,
        FLOOR      = 32,
        TRAP       = 64,
        DROPPED    = 128,
        BLOCK = REAL | WALL,
        Any = ~0,
    }
}