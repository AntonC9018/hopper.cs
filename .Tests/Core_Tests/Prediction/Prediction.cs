using System.Linq;
using Hopper.Core;
using Hopper.Core.Behaviors.Basic;
using Hopper.Core.Mods;
using Hopper.Core.Predictions;
using Hopper.Test_Content.Explosion;
using Hopper.Test_Content.SimpleMobs;
using Hopper.Utils.Vector;
using NUnit.Framework;

namespace Hopper.Tests
{
    public class Prediction_Tests
    {
        public ModResult result;
        public EntityFactory<Player> playerFactory;
        public World world;
        public Predictor prediction;

        public Prediction_Tests()
        {
            result = SetupThing.SetupContent();
            playerFactory = new EntityFactory<Player>();
        }

        [SetUp]
        public void Setup()
        {
            world = new World(3, 3, result.patchArea);
            prediction = new Predictor(world, Faction.Player);
        }

        [Test]
        public void WithoutEnemies_ThereAreNoBadCells()
        {
            var player = world.SpawnPlayer(playerFactory, new IntVector2(0, 0));
            Assert.Zero(prediction.GetBadPositions().Count());
        }

        [Test]
        public void WithEnemyTryingToAttack_ThereAreBadCells()
        {
            var skeleton = world.SpawnEntity(Skeleton.Factory, new IntVector2(1, 1));
            var acting = skeleton.Behaviors.Get<Acting>();
            acting.CalculateNextAction();

            // Assert.True(acting.NextAction.(typeof(BehaviorAction<Attacking>)), "Will attack");

            var player = world.SpawnPlayer(playerFactory, new IntVector2(1, 0));

            // Just in case, assert that the faction works correctly
            Assert.True(player.IsPlayer, "IsPlayer says we are player");

            var predictedPositions = prediction.GetBadPositions();
            Assert.AreEqual(1, predictedPositions.Count());
            Assert.AreEqual(new IntVector2(1, 0), predictedPositions.First());

            player.ResetPosInGrid(new IntVector2(0, 0));
            predictedPositions = prediction.GetBadPositions().ToList();
            Assert.AreEqual(2, predictedPositions.Count());
            Assert.True(predictedPositions.Contains(new IntVector2(1, 0)));
            Assert.True(predictedPositions.Contains(new IntVector2(0, 1)));
        }

        [Test]
        public void ExplosionPredictionWorks()
        {
            var entity = world.SpawnEntity(Dummy.Factory, new IntVector2(1, 1));
            var predictions = Explosion.DefaultExplodeAction(1).predict(entity).ToArray();
            Assert.AreEqual(9, predictions.Length, "Would explode 9 cells");
        }
    }
}