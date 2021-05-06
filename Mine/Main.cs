using System;
using System.Collections.Generic;
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

            World.Global = new World(3, 3);
            var world = World.Global;

            var entityFactory = new EntityFactory();
            Transform.AddTo(entityFactory, Layer.REAL);
            Stats.AddTo(entityFactory, Registry.Global._defaultStats);
            Displaceable.AddTo(entityFactory, ExtendedLayer.BLOCK).DefaultPreset();
            Pushable.AddTo(entityFactory).DefaultPreset();
            Stats.AddInitTo(entityFactory);

            var entity = World.Global.SpawnEntity(entityFactory, new IntVector2(0, 1));
            var trap1 = World.Global.SpawnEntity(BounceTrap.Factory, new IntVector2(1, 1), new IntVector2(1, 0));
            entity.Displace(new IntVector2(1, 0), Move.Default());
            World.Global.Loop();

            var trap2 = World.Global.SpawnEntity(BounceTrap.Factory, new IntVector2(2, 1), new IntVector2(1, 0));
            entity.GetTransform().ResetPositionInGrid(new IntVector2(0, 1));
            World.Global.Loop();
            entity.Displace(new IntVector2(1, 0), Move.Default());
            World.Global.Loop();
        }
    }
}