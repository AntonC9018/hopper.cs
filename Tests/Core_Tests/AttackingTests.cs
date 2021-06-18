using System.Linq;
using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Targeting;
using Hopper.Utils.Vector;
using NUnit.Framework;
using Hopper.Core.WorldNS;
using Hopper.Core.ActingNS;
using Hopper.Core.Stat;
using Hopper.Utils.Chains;

namespace Hopper.Tests
{
    public class AttackingTests
    {
        public EntityFactory attackerFactory;
        public EntityFactory attackedFactory;

        public AttackingTests()
        {
            InitScript.Init();

            // attacking requires stats and transform
            attackerFactory = new EntityFactory();
            Transform.AddTo(attackerFactory, Layers.REAL, 0);
            Stats.AddTo(attackerFactory, Registry.Global.Stats._map); 
            Attacking.AddTo(attackerFactory, entity => BufferedAttackTargetProvider.Simple, Layers.REAL, Faction.Any).SkipEmptyAttackPreset();
            

            attackedFactory = new EntityFactory();
            Transform.AddTo(attackedFactory, Layers.REAL, 0);
            FactionComponent.AddTo(attackedFactory, Faction.Environment);
            Stats.AddTo(attackedFactory, Registry.Global.Stats._map); 
            Attackable.AddTo(attackedFactory, Attackness.ALWAYS);
        }

        [SetUp]
        public void Setup()
        {
            World.Global = new World(3, 3);
        } 

        [Test]
        public void AttackTest()
        {
            var attacker = World.Global.SpawnEntity(attackerFactory, new IntVector2(0, 0));
            Assert.False(attacker.Attack(IntVector2.Right));
            Assert.AreEqual(new IntVector2(1, 0), attacker.GetAttacking()
                .Predict(attacker, IntVector2.Right, 
                    new PredictionTargetInfo(Layers.REAL, Faction.Environment))
                .Single());
            
            var attacked = World.Global.SpawnEntity(attackedFactory, new IntVector2(1, 0));
            
            bool pain = false;
            var painHandler = new Handler<Attackable.Context>(ctx => pain = true);
            Attackable.AfterPath.Get(attacked).Add(painHandler);

            bool did = false;
            var didHandler = new Handler<Attacking.Context>(ctx => did = true);
            Attacking.AfterPath.Get(attacker).Add(didHandler);

            Assert.True(attacker.Attack(IntVector2.Right));
            Assert.True(pain);
            Assert.True(did);

            attacked.Die();
            pain = false;
            did = false;

            Assert.False(attacker.Attack(IntVector2.Right));
            Assert.False(pain);
            Assert.False(did);
        }
    }
}