using System.Collections.Generic;
using Hopper.Core.Stats;
using Hopper.Utils;

namespace Hopper.Core.Registry
{
    public class Repository
    {
        public DefaultStats DefaultStats;
        private Dictionary<System.Type, IPatchSubRegistry<IPatch>> PatchRegistries;

        public Repository()
        {
            DefaultStats = new DefaultStats(this);
            PatchRegistries = new Dictionary<System.Type, IPatchSubRegistry<IPatch>>();
        }

        public PatchSubRegistry<T> GetPatchSubRegistry<T>()
            where T : IPatch
        {
            if (!PatchRegistries.ContainsKey(typeof(T)))
            {
                System.Console.WriteLine($"A PatchRegistry of type {typeof(T)} was not found. Creating one.");
                PatchRegistries.Add(typeof(T), (IPatchSubRegistry<IPatch>)new PatchSubRegistry<T>());
            }
            return (PatchSubRegistry<T>)PatchRegistries[typeof(T)];
        }

        public T GetCustomPatchRegistry<T, U>()
            where T : IPatchSubRegistry<U>
            where U : IPatch
        {
            Assert.That(PatchRegistries.ContainsKey(typeof(U)),
                $"Patch subregistry for {typeof(U)} of the requested type {typeof(T).Name} not found");
            return (T)PatchRegistries[typeof(U)];
        }

        public void AddCustomPatchRegistry<T>(IPatchSubRegistry<IPatch> reg)
            where T : IPatch
        {
            System.Console.WriteLine($"Setting a custom patch subregistry for type {typeof(T)}");
            PatchRegistries.Add(typeof(T), reg);
        }
    }
}