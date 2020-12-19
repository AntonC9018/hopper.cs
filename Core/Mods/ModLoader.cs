using System.Collections.Generic;
using Hopper.Core.Registries;

namespace Hopper.Core.Mods
{
    public class ModResult
    {
        public ModsContent mods;
        public Registries.Registry registry;
        public PatchArea patchArea;
    }

    public class ModLoader
    {
        public List<System.Type> modTypes;

        public ModLoader()
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
            Registries.Registry registry = new Registries.Registry();

            // Run the `Kind` phase
            foreach (System.Type modType in modTypes)
            {
                System.Console.WriteLine($"Registering kinds for mod {modType.Name}...");
                var mod = mods.m_mods[modType];
                ModRegistry modSubRegistry = registry.CreateModRegistry(mod);
                mod.RegisterSelf(modSubRegistry);
            }

            PatchArea patchArea = new PatchArea(registry);

            // Run the `Pre_Patching` phase
            foreach (System.Type modType in modTypes)
            {
                System.Console.WriteLine($"Running Pre_Patching for mod {modType.Name}...");
                var mod = mods.m_mods[modType];
                mod.PrePatch(patchArea);
            }

            // Run the `Patching` phase
            foreach (System.Type modType in modTypes)
            {
                System.Console.WriteLine($"Running Patching for mod {modType.Name}...");
                var mod = mods.m_mods[modType];
                mod.Patch(patchArea);
            }

            // Run the `Post_Patching` phase
            foreach (System.Type modType in modTypes)
            {
                System.Console.WriteLine($"Running Post_Patching for mod {modType.Name}...");
                var mod = mods.m_mods[modType];
                mod.PostPatch(patchArea);
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