using Hopper.Core.Mods;
using Hopper.Test_Content;

namespace Hopper.Tests
{
    public static class SetupThing
    {
        private static bool isLoaded;

        public static void SetupContent()
        {
            var loader = new ModLoader();
            loader.Init();
        }
    }
}