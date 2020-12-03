using System.Collections.Generic;
using Core.Items;
using Core.Stats.Basic;

namespace Core
{
    public struct FactoryLink
    {
        public int factoryId;
    }

    public class Registry
    {
        public static Registry Default = new Registry();

        public bool IsInRuntimePhase { get; private set; } = false;

        public InstanceRegistry<Entity, FactoryLink> Entity = new InstanceRegistry<Entity, FactoryLink>();
        public InstanceRegistry<World> World = new InstanceRegistry<World>();

        public KindRegistry<ITinker> Tinker => GetKindRegistry<ITinker>();
        public KindRegistry<Retoucher> Retoucher => GetKindRegistry<Retoucher>();
        public KindRegistry<IFactory<Entity>> EntityFactory => GetKindRegistry<IFactory<Entity>>();
        public KindRegistry<IItem> Items => GetKindRegistry<IItem>();
        public KindRegistry<Attack.Source> AttackSources => GetKindRegistry<Attack.Source>();
        public KindRegistry<Push.Source> PushSources => GetKindRegistry<Push.Source>();

        private Dictionary<System.Type, IKindRegistry> All;

        public Registry()
        {
            All = new Dictionary<System.Type, IKindRegistry>
            {
                { typeof(ITinker), new KindRegistry<ITinker>() },
                { typeof(Retoucher), new KindRegistry<Retoucher>()},
                { typeof(IFactory<Entity>), new KindRegistry<IFactory<Entity>>() },
                { typeof(IItem), new KindRegistry<IItem>() },
                { typeof(Attack.Source), new KindRegistry<Attack.Source>() },
                { typeof(Push.Source), new KindRegistry<Push.Source>() },
                { typeof(IWorldEvent), new KindRegistry<IWorldEvent>() }
            };
        }

        public T GetKind<T>(int id) where T : IHaveId
        {
            return ((KindRegistry<T>)All[typeof(T)]).Map(id);
        }

        public KindRegistry<T> GetKindRegistry<T>() where T : IHaveId
        {
            return ((KindRegistry<T>)All[typeof(T)]);
        }
    }
}