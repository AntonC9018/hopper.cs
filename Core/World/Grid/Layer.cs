using Hopper.Shared.Attributes;

namespace Hopper.Core.WorldNS
{

    // This indicates the order in which actions are executed
    [Flags] public enum Layer
    {
        REAL = 0b_0000_0001,
        MISC = 0b_0000_0010,
        WALL = 0b_0000_0100,
        PROJECTILE = 0b_0000_1000,
        ITEM = 0b_0001_0000,
        FLOOR = 0b_0010_0000,
        TRAP = 0b_0100_0000,
        DROPPED = 0b_1000_0000,
        Any = ~0
    }

    public static class ExtendedLayer
    {
        public static Layer BLOCK = Layer.REAL | Layer.WALL;
    }

    public static class LayerExtensions
    {
        public static string GetName(this Layer layer)
        {
            return System.Enum.GetName(typeof(Layer), layer);
        }

        public static Layer ToLayer(this int num)
        {
            return (Layer)(1 << (num - 1));
        }

        public static int ToIndex(this Layer layer)
        {
            int i = 0;
            uint num = (uint)layer;

            while ((num >>= 1) != 0)
                i++;

            return i;
        }
    }
}