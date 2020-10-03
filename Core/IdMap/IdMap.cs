using System.Collections.Generic;
using Core.Items;
using Newtonsoft.Json;

namespace Core
{
    public struct FactoryAndWorldIds
    {
        public int factoryId;
    }

    public static class IdMap
    {
        public static bool IsInRuntimePhase { get; private set; } = false;


        public static RuntimeIdMap<Entity, FactoryAndWorldIds> Entity =
            new RuntimeIdMap<Entity, FactoryAndWorldIds>();

        public static RuntimeIdMap<World> World = new RuntimeIdMap<World>();

        public static SetupIdMap<ITinker> Tinker = new SetupIdMap<ITinker>();
        public static SetupIdMap<Retoucher> Retoucher = new SetupIdMap<Retoucher>();
        public static SetupIdMap<IEntityFactory> EntityFactory = new SetupIdMap<IEntityFactory>();
        public static SetupIdMap<IItem> Items = new SetupIdMap<IItem>();
        public static SetupIdMap<IStatus> Status = new SetupIdMap<IStatus>();
        public static SetupIdMap<Attack.Source> AttackSources = new SetupIdMap<Attack.Source>();
        public static SetupIdMap<Push.Source> PushSources = new SetupIdMap<Push.Source>();

        public static Dictionary<System.Type, ISetupIdMap> All = new Dictionary<System.Type, ISetupIdMap>
        {
            { typeof(ITinker), Tinker },
            { typeof(Retoucher), Retoucher},
            { typeof(IEntityFactory), EntityFactory },
            { typeof(IItem), Items },
            { typeof(IStatus), Status},
            { typeof(Attack.Source), AttackSources },
            { typeof(Push.Source), PushSources }
        };

        public static T Map<T>(int id) where T : IHaveId
        {
            return ((SetupIdMap<T>)All[typeof(T)]).Map(id);
        }
    }
}