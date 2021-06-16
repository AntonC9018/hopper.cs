
using System.Collections.Generic;
using System.Linq;
using Hopper.Core;
using Hopper.Core.ActingNS;
using Hopper.Core.Components.Basic;
using Hopper.Core.Mods;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.Core.WorldNS;
using Hopper.TestContent.BindingNS;
using Hopper.Utils.Vector;

namespace Mine
{
    public class Program
    {
        public static EntityFactory attackerFactory;

        public static void Main()
        {
            Hopper.Core.Main.Init();
            Hopper.TestContent.Main.Init();

            attackerFactory = new EntityFactory();
            Transform.AddTo(attackerFactory, Layers.REAL, 0);
            Stats.AddTo(attackerFactory, Registry.Global.Stats._map); 
            Attacking.AddTo(attackerFactory, entity => BufferedAttackTargetProvider.Simple, Layers.REAL, Faction.Any).SkipEmptyAttackPreset();
            World.Global = new World(3, 3);
            var attacker = World.Global.SpawnEntity(attackerFactory, new IntVector2(0, 0));
            attacker.Attack(IntVector2.Right);
        }
    }
}