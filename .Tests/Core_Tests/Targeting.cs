using System.Linq;
using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Targeting;
using Hopper.Utils.Vector;
using NUnit.Framework;

namespace Hopper.Tests
{
    public class TargetingTests
    {
        public EntityFactory wallFactory;
        public EntityFactory entityFactory;

        public TargetingTests()
        {
            InitScript.Init();

            wallFactory = new EntityFactory();
            Transform.AddTo(wallFactory, Layer.WALL);
            Attackable.AddTo(wallFactory, Attackness.IS_BLOCK);

            entityFactory = new EntityFactory();
            Transform.AddTo(entityFactory, Layer.REAL);
            Attackable.AddTo(entityFactory, Attackness.ALWAYS);
        }

        public GridManager Grid => World.Global.grid;

        [SetUp]
        public void Setup()
        {
            World.Global = new World(3, 3);
        }

        [Test]
        public void Buffered()
        {
            var pattern = new PieceAttackPattern(
                new Piece(new IntVector2(1, 0), new IntVector2(1, 0), new Reach(true))
            );
            var provider = new BufferedAttackTargetProvider(pattern, 
                BufferedAttackTargetProvider.SingleSimpleMap, Layer.REAL, 0);
            var entity = World.Global.SpawnEntity(entityFactory, new IntVector2(0, 0));

            AttackTargetingContext context;

            // 1. "Dagger" via our own pattern
            context = provider.GetTargets(null, new IntVector2(0, 1), new IntVector2(0, -1));
            Assert.AreSame(context.targetContexts.Single().transform, entity.GetTransform());

            // 2. "Dagger" via the default simple target provider
            context = BufferedAttackTargetProvider.Simple.GetTargets(null, new IntVector2(0, 1), new IntVector2(0, -1));
            Assert.AreSame(context.targetContexts.Single().transform, entity.GetTransform());

            // 3. "Dagger" via SingleDefaultMap and a custom pattern
            provider = new BufferedAttackTargetProvider(pattern,
                BufferedAttackTargetProvider.SingleDefaultMap, Layer.REAL, 0);
            context = provider.GetTargets(null, new IntVector2(0, 1), new IntVector2(0, -1));
            Assert.AreSame(context.targetContexts.Single().transform, entity.GetTransform());

            // 4. "Spear" via SingleDefaultMap and a custom pattern
            pattern = new PieceAttackPattern(
                new Piece(new IntVector2(1, 0), new IntVector2(1, 0), new Reach(true)),
                new Piece(new IntVector2(2, 0), new IntVector2(1, 0), new Reach(0))
            );
            provider = new BufferedAttackTargetProvider(pattern,
                BufferedAttackTargetProvider.SingleDefaultMap, Layer.REAL, Layer.WALL);

            // Targeting an entity 2 blocks away
            context = provider.GetTargets(null, new IntVector2(0, 2), new IntVector2(0, -1));
            Assert.AreSame(context.targetContexts.Single().transform, entity.GetTransform());

            // Targeting an entity 1 block away
            context = provider.GetTargets(null, new IntVector2(0, 1), new IntVector2(0, -1));
            Assert.AreSame(context.targetContexts.Single().transform, entity.GetTransform());

            // Targeting an entity 2 blocks away being blocked by a wall
            var wall = World.Global.SpawnEntity(wallFactory, new IntVector2(0, 1));
            context = provider.GetTargets(null, new IntVector2(0, 2), new IntVector2(0, -1));
            Assert.That(context.targetContexts.Count == 0);

            // If the wall can be attacked but not at default, attack should go through it
            // Given the entity is not a block at the same time.
            wall.GetAttackable()._attackness = Attackness.SKIP;
            context = provider.GetTargets(null, new IntVector2(0, 2), new IntVector2(0, -1));
            Assert.AreSame(context.targetContexts.Single().transform, entity.GetTransform());

            // If it blocks, and can be attacked, but not by default, then it should block the attack
            wall.GetAttackable()._attackness = Attackness.MAYBE;
            context = provider.GetTargets(null, new IntVector2(0, 2), new IntVector2(0, -1));
            Assert.That(context.targetContexts.Count == 0);

            wall.GetTransform().RemoveFromGrid();

            // The entity that can only be attacked whil next to it, should not be attacked
            // even if the weapon reaches from 2 spaces away, assuming the default map.
            entity.GetAttackable()._attackness = Attackness.CAN_BE_ATTACKED_IF_NEXT_TO;
            context = provider.GetTargets(null, new IntVector2(0, 2), new IntVector2(0, -1));
            Assert.That(context.targetContexts.Count == 0);

            // If it is next to the position, the attack should go through
            context = provider.GetTargets(null, new IntVector2(0, 1), new IntVector2(0, -1));
            Assert.AreSame(context.targetContexts.Single().transform, entity.GetTransform());
        }
    }
}