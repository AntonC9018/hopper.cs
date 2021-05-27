using Hopper.Core;
using Hopper.Core.Mods;
using Hopper.Utils.Vector;
using NUnit.Framework;

namespace Hopper.Tests
{
    public static class InitScript
    {
        public static ModLoader modLoader;

        public static void Init()
        { 
            if (modLoader != null) return;
            modLoader = new ModLoader();
            modLoader.Add(Core.Main.Init);
            modLoader.Add(TestContent.Main.Init);
            modLoader.Init();
        }
    }
}