using System;
using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Items;
using Hopper.Core.Predictions;
using Hopper.Core.Retouchers;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.TestContent;
using Hopper.TestContent.Bind;
using Hopper.TestContent.Floor;
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

            var entityFactory = new EntityFactory();
            Transform.AddTo(entityFactory, Layer.REAL);
            Stats.AddTo(entityFactory, Registry.Global._defaultStats);
            Ticking.AddTo(entityFactory).DefaultPreset();
            Displaceable.AddTo(entityFactory, ExtendedLayer.BLOCK).DefaultPreset();
            Moving.AddTo(entityFactory).DefaultPreset();
            Pushable.AddTo(entityFactory).DefaultPreset();
            Acting.AddTo(entityFactory, null, Algos.SimpleAlgo, Order.Entity).DefaultPreset(entityFactory);
            entityFactory.InitInWorldFunc += t => t.entity.GetStats().Init();

            var world = new World(4, 4);
            World.Global = world;
            var _iceFloors = new Entity[3];
            for (int i = 0; i < 3; i++)
            {
                _iceFloors[i] = World.Global.SpawnEntity(IceFloor.Factory, new IntVector2(i, 1));
            }

            var entity = World.Global.SpawnEntity(entityFactory, new IntVector2(0, 1));
            entity.Move(IntVector2.Right);

            var acting = entity.GetActing();
            acting.nextAction = Moving.Action.Compile(IntVector2.Up);
            acting.Activate();
            acting.Activate();
            acting.Activate();
        }
    }
}