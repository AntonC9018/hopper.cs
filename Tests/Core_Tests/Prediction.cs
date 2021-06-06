using System.Linq;
using Hopper.Core;
using Hopper.Core.ActingNS;
using Hopper.Core.Targeting;
using Hopper.Core.WorldNS;
using Hopper.TestContent;
using Hopper.TestContent.SimpleMobs;
using Hopper.Utils.Vector;
using NUnit.Framework;

namespace Hopper.Tests
{
    public class Prediction_Tests
    {
        public Prediction_Tests()
        {
            InitScript.Init();
        }

        [SetUp]
        public void Setup()
        {
            World.Global = new World(3, 3);
        }

        [Test]
        public void WithoutEnemies_ThereAreNoBadCells()
        {
            var player = World.Global.SpawnEntity(Player.Factory, new IntVector2(0, 0));
            var predictor = new Predictor(World.Global, Layers.REAL, Faction.Player);
            Assert.Zero(predictor.GetBadPositions().Count());
        }

        [Test]
        public void WithEnemyTryingToAttack_ThereAreBadCells()
        {
            var skeleton = World.Global.SpawnEntity(Zombie.Factory, new IntVector2(1, 1));
            var acting = skeleton.GetActing();
            acting.CalculateAndSetAction();

            Assert.NotNull(acting.nextAction, "Will attack");

            var player = World.Global.SpawnEntity(Player.Factory, new IntVector2(1, 0));
            var predictor = new Predictor(World.Global, Layers.REAL, Faction.Player);
            
            // Just in case, assert that the faction works correctly
            Assert.True(player.IsPlayer(), "IsPlayer says we are player");

            var predictedPositions = predictor.GetBadPositions();
            Assert.AreEqual(1, predictedPositions.Count());
            Assert.AreEqual(new IntVector2(1, 0), predictedPositions.First());

            player.GetTransform().ResetPositionInGrid(new IntVector2(0, 0));
            predictedPositions = predictor.GetBadPositions();
            Assert.AreEqual(2, predictedPositions.Count());
            Assert.True(predictedPositions.Contains(new IntVector2(1, 0)));
            Assert.True(predictedPositions.Contains(new IntVector2(0, 1)));
        }

        [Test]
        public void ExplosionPredictionWorks()
        {
            var entity = World.Global.SpawnEntity(Dummy.Factory, new IntVector2(1, 1));
            var info = new PredictionTargetInfo(Layers.Any, Faction.Any);
            var predictions = Explosion.DefaultExplodeAction(1).Predict(entity, info).ToArray();
            Assert.AreEqual(9, predictions.Length, "Would explode 9 cells");
        }

        [Test]
        public void ProviderPredictionWorks()
        {
            var pattern  = new StraightPattern(Layers.WALL);
            var provider = new UnbufferedTargetProvider(pattern, Layers.REAL, Faction.Any); 

            // Generating predictions relative to this point
            var info = new PredictionTargetInfo(Layers.REAL, Faction.Player);

            var positions = provider.PredictPositions(IntVector2.Zero, IntVector2.Right, info);

            Assert.AreEqual(2, positions.Count());

            var wall = World.Global.SpawnEntity(Wall.Factory, new IntVector2(1, 0));
            positions = provider.PredictPositions(IntVector2.Zero, IntVector2.Right, info);

            Assert.AreEqual(0, positions.Count());

            wall.GetTransform().ResetPositionInGrid(new IntVector2(2, 0));
            positions = provider.PredictPositions(IntVector2.Zero, IntVector2.Right, info);

            Assert.AreEqual(1, positions.Count());
        }
    }
}