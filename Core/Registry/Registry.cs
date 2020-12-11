using System.Collections.Generic;
using Hopper.Core.Items;
using Hopper.Core.Stats;
using Hopper.Core.Stats.Basic;

namespace Hopper.Core
{
    public struct FactoryLink
    {
        public int factoryId;
    }

    public class Registry
    {
        public bool IsInRuntimePhase { get; private set; } = false;

        public InstanceRegistry<Entity, FactoryLink> Entity = new InstanceRegistry<Entity, FactoryLink>();
        public InstanceRegistry<World> World = new InstanceRegistry<World>();

        public KindRegistry<ITinker> Tinker => GetKindRegistry<ITinker>();
        public KindRegistry<Retoucher> Retoucher => GetKindRegistry<Retoucher>();
        public KindRegistry<IFactory<Entity>> EntityFactory => GetKindRegistry<IFactory<Entity>>();
        public KindRegistry<IItem> Items => GetKindRegistry<IItem>();
        private Dictionary<System.Type, IKindRegistry<IKind>> All;
        public DefaultStats DefaultStats;

        public event System.Action<Registry> RunPatchingEvent;

        public void RunPathing()
        {
            RunPatchingEvent?.Invoke(this);
        }

        public Registry()
        {
            All = new Dictionary<System.Type, IKindRegistry<IKind>>
            {
                { typeof(ITinker), new KindRegistry<ITinker>() },
                { typeof(Retoucher), new KindRegistry<Retoucher>()},
                { typeof(IFactory<Entity>), new KindRegistry<IFactory<Entity>>() },
                { typeof(IItem), new KindRegistry<IItem>() },
                { typeof(IWorldEvent), new KindRegistry<IWorldEvent>() }
            };
        }

        // temporary entityFactory -> defaultStats map
        public PatchRegistry<DefaultStats> EntityFactoryPatch = new PatchRegistry<DefaultStats>();
        private Dictionary<System.Type, IPatchRegistry<object>> PatchRegistries;
        private Dictionary<System.Type, JustAssignmentRegistry> AssignmentRegistries;
        public Dictionary<object, int> IdReferences = new Dictionary<object, int>();


        // public StatTree m_statTree = new StatTree();


        public T GetKind<T>(int id) where T : IKind
        {
            return ((KindRegistry<T>)All[typeof(T)]).Get(id);
        }

        public KindRegistry<T> GetKindRegistry<T>() where T : IKind
        {
            return ((KindRegistry<T>)All[typeof(T)]);
        }

        public PatchRegistry<T> GetPatchRegistry<T, U>()
            where T : IPatch
            where U : IKind
        {
            return (PatchRegistry<T>)PatchRegistries[typeof(U)];
        }

        public T GetCustomPatchRegistry<T, U>() where T : IPatchRegistry<object>
        {
            return (T)PatchRegistries[typeof(U)];
        }

        public T AddCustomPatchRegistry<T, U>(T reg) where T : IPatchRegistry<object>
        {
            PatchRegistries[typeof(U)] = reg;
        }

        public JustAssignmentRegistry GetAssignmentRegistry<T>()
        {
            return (JustAssignmentRegistry)AssignmentRegistries[typeof(T)];
        }
    }
}