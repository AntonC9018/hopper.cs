using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Items;
using Hopper.Core.Predictions;
using Hopper.Core.Retouchers;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.TestContent;
using Hopper.TestContent.SimpleMobs;
using Hopper.Utils;
using Hopper.Utils.Vector;

namespace Hopper.Mine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Hopper.Core.Main.Init();
            Hopper.TestContent.Main.Init();

    
            World.Global = new World(3, 3);

            var skeleton = World.Global.SpawnEntity(Skeleton.Factory, new IntVector2(1, 1));
            var acting = skeleton.GetActing();
            acting.CalculateNextAction();

            var predictor = new Predictor(World.Global, Faction.Player);
            var player = World.Global.SpawnEntity(Player.Factory, new IntVector2(1, 0));
            var predictedPositions = predictor.GetBadPositions();
        }
    }
}