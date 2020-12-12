using System.Collections.Generic;

namespace Hopper.Core.Mods
{
    public class ModManager
    {
        public List<System.Type> modTypes;

        public ModManager()
        {
            modTypes = new List<System.Type>();
            // add the indispensable one right away
            Add<CoreMod>();
        }

        public void Add<T>() where T : IMod
        {
            modTypes.Add(typeof(T));
        }

        public Registry RegisterAll()
        {
            // Run the `Content` phase
            ModsContent mods = new ModsContent();
            foreach (System.Type modType in modTypes)
            {
                mods.m_mods[modType] = (IMod)System.Activator.CreateInstance(modType, mods);
            }

            // Prepare the registry
            Registry registry = new Registry();
            registry.ModContent = mods;

            // Run the `Kind` phase
            foreach (System.Type modType in modTypes)
            {
                mods.m_mods[modType].RegisterSelf(registry);
            }

            // Run the `Patching` phase
            registry.RunPatching();

            // TODO: somehow signal to start the `Instance` phase

            return registry;
        }
    }
}