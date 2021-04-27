using Hopper.Core;
using Hopper.Shared.Attributes;

namespace Hopper.TestContent.Freezing
{
    [EntityType(Abstract = true)]
    public static class SimpleEssentials
    {
        public static EntityFactory Factory;

        public static void AddComponents(Entity subject)
        {
        }

        public static void InitComponents(Entity subject)
        {
        }
    }
}