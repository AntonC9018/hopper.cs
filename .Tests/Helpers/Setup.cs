using Hopper.Core.Mods;
using Hopper.Test_Content;

namespace Hopper.Tests
{
    public static class SetupThing
    {
        static ModResult result;

        public static ModResult SetupContent()
        {
            if (result == null)
            {
                var loader = new ModLoader();
                loader.Add<TestMod>();
                result = loader.RegisterAll();
            }
            return result;
        }
    }
}