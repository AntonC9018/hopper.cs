using System.Collections.Generic;

namespace Hopper.Core.Mods
{
    public class ModLoader
    {
        public List<System.Action> modInits;

        public ModLoader()
        {
            modInits.Add(Main.Init);
        }

        public void Add(System.Action modInit)
        {
            modInits.Add(modInit);
        }

        public void Init()
        {
            foreach (var mod in modInits)
            {
                mod();
            }
        }
    }
}