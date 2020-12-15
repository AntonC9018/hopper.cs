using System.Collections.Generic;
using Hopper.Core.Registry;

namespace Hopper.Core.Mods
{
    public class ModResult
    {
        public ModsContent mods;
        public Registry.Registry registry;
        public PatchArea patchArea;
    }

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

        public ModResult RegisterAll()
        {
            // Run the `Content` phase
            ModsContent mods = new ModsContent();
            foreach (System.Type modType in modTypes)
            {
                System.Console.WriteLine($"Creating content for mod {modType.Name}...");
                mods.m_mods[modType] = (IMod)System.Activator.CreateInstance(modType);
            }

            // Prepare the registry
            Registry.Registry registry = new Registry.Registry();

            // Run the `Kind` phase
            foreach (System.Type modType in modTypes)
            {
                System.Console.WriteLine($"Registering kinds for mod {modType.Name}...");
                var mod = mods.m_mods[modType];
                ModRegistry modSubRegistry = registry.CreateModRegistry(mod);
                mod.RegisterSelf(modSubRegistry);
            }

            PatchArea patchArea = new PatchArea();

            // Run the `Patching` phase
            foreach (System.Type modType in modTypes)
            {
                System.Console.WriteLine($"Running patching for mod {modType.Name}...");
                var mod = mods.m_mods[modType];
                mod.Patch(patchArea);
            }

            // Run the `AfterPatch` phase
            foreach (System.Type modType in modTypes)
            {
                System.Console.WriteLine($"Running after_patching for mod {modType.Name}...");
                var mod = mods.m_mods[modType];
                mod.Patch(patchArea);
            }

            return new ModResult
            {
                mods = mods,
                registry = registry,
                patchArea = patchArea,
            };
        }
    }
}