using System.Collections.Generic;
using Hopper.Core.Stats;
using Hopper.Utils;

namespace Hopper.Core.Registries
{
    public class PatchArea
    {
        public DefaultStats DefaultStats;
        private Dictionary<System.Type, IPatchSubArea<IPatch>> PatchRegistries;
        public readonly Registry registry;

        public PatchArea(Registry parentRegistry)
        {
            DefaultStats = new DefaultStats(this);
            PatchRegistries = new Dictionary<System.Type, IPatchSubArea<IPatch>>();
            registry = parentRegistry;
        }

        public PatchSubArea<T> GetPatchSubRegistry<T>()
            where T : IPatch
        {
            if (!PatchRegistries.ContainsKey(typeof(T)))
            {
                System.Console.WriteLine($"A Patch Subarea of type {typeof(T)} was not found. Creating one.");
                PatchRegistries.Add(typeof(T), (IPatchSubArea<IPatch>)new PatchSubArea<T>());
            }
            return (PatchSubArea<T>)PatchRegistries[typeof(T)];
        }

        public T GetCustomPatchRegistry<T, U>()
            where T : IPatchSubArea<U>
            where U : IPatch
        {
            Assert.That(PatchRegistries.ContainsKey(typeof(U)),
                $"Patch Subarea for {typeof(U)} of the requested type {typeof(T).Name} not found");
            return (T)PatchRegistries[typeof(U)];
        }

        public void AddCustomPatchRegistry<T>(IPatchSubArea<IPatch> reg)
            where T : IPatch
        {
            System.Console.WriteLine($"Setting a custom Patch Subarea for type {typeof(T)}");
            PatchRegistries.Add(typeof(T), reg);
        }
    }
}