using System;
using System.Collections.Generic;
using Hopper.Core;
using Hopper.Core.ActingNS;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Items;
using Hopper.Core.Retouchers;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.Core.WorldNS;
using Hopper.TestContent;
using Hopper.TestContent.BindingNS;
using Hopper.TestContent.PinningNS;
using Hopper.TestContent.SimpleMobs;
using Hopper.Utils;
using Hopper.Utils.Vector;
using static Hopper.Utils.Vector.IntVector2;

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


            var template = new StatsBuilder
            {
                { Attack.Index,                                  new Attack(1, 2, 3, Attack.Source.Basic.Index) },
                { Push.Index,                                    new Push(1, 2, 3, Push.Source.Basic.Index)     },
                { Push.Source.Basic.Index,                       new Push.Source.Resistance(5)                  },
                { Attack.Source.Basic.Index,                     new Attack.Source.Resistance(4)                },
                { TestContent.Stat.Explosion.AttackSource.Index, new Attack.Source.Resistance(3)                },
                { Attack.Resistance.Index,                       new Attack.Resistance(1, 2, 3, 4)              },
                { Push.Resistance.Index,                         new Push.Resistance(5)                         },
                { Dig.Index,                                     new Dig(6, 7, 8)                               },
                { TestContent.Stat.Bind.Source.Index,            new StatusSource.Resistance(9)                 }
                // 3, 9, 5
            };


            Stats.AddTo(entityFactory, Registry.Global._defaultStats);
            Stats.AddInitTo(entityFactory);

            var stats = entityFactory.subject.GetStats();

            stats.GetLazy(Attack.Index, out var attack);

            var factory = Player.Factory;
            Equip.OnDisplaceHandlerWrapper.HookTo(factory);
        }
    }
}