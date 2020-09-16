using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Behaviors;
using Core.Items;
using Utils;

namespace Core
{
    public static class IdMap
    {
        // public
        public static bool IsInRuntimePhase { get; private set; } = false;

        public static RuntimeIdMap<Entity, IEntityFactory> Entity =
            new RuntimeIdMap<Entity, IEntityFactory>();

        public static SetupIdMap<ITinker> Tinker = new SetupIdMap<ITinker>();
        public static SetupIdMap<Retoucher> Retoucher = new SetupIdMap<Retoucher>();
        public static SetupIdMap<IEntityFactory> EntityFactory = new SetupIdMap<IEntityFactory>();
        public static SetupIdMap<IItem> Items = new SetupIdMap<IItem>();
        public static SetupIdMap<IStatus> Status = new SetupIdMap<IStatus>();
        public static SetupIdMap<Attack.Source> AttackSources = new SetupIdMap<Attack.Source>();
        public static SetupIdMap<Push.Source> PushSources = new SetupIdMap<Push.Source>();

        // static IdMap()
        // {
        //     Items.SetGlobalMap(Items.PackModMap());
        //     Tinker.SetGlobalMap(Tinker.PackModMap());
        //     Status.SetGlobalMap(Status.PackModMap());
        //     Retoucher.SetGlobalMap(Retoucher.PackModMap());
        //     EntityFactory.SetGlobalMap(EntityFactory.PackModMap());
        // }
    }
}