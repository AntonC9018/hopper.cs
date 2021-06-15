using Hopper.Core.WorldNS;
using Hopper.Shared.Attributes;

namespace Hopper.Core
{
    [ExportingClass]
    public static partial class QueriesStuff
    {
        // TODO: idk about this one...
        [Export(Chain = "gWorld.SpawnEntity")]
        public static void UpdateQueries(Entity entity)
        {
            Registry.Global.Queries.Faction.AddEntity(entity);
        }
    }


    public struct Queries
    {
        public FactionQuery Faction;

        public void Init()
        {
            Faction.Init();
            World.SpawnEntityDefaultChain.Add(QueriesStuff.UpdateQueriesHandler);
        }
    }
}