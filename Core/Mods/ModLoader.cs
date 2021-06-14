using System.Collections.Generic;

namespace Hopper.Core.Mods
{
    public class ModLoader
    {
        public List<System.Action> modInits;

        public ModLoader()
        {
            modInits = new List<System.Action>();
        }

        public void Add(System.Action modInit)
        {
            modInits.Add(modInit);
        }

        public void InitMods()
        {
            foreach (var mod in modInits)
            {
                mod();
            }
            Registry.Global.AfterInit();
        }
    }
}