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
    public class ControllableTests
    {
        public ControllableTests()
        {
            InitScript.Init();
        }

        [SetUp]
        public void Setup()
        {
            World.Global = new World(2, 2);
        }

        [Test]
        public void VectorInputWorks()
        {
            var player = World.Global.SpawnEntity(Player.Factory, new IntVector2(0, 0));
            
            var controllable = player.GetControllable();
            var acting       = player.GetActing();

            Assert.That(acting.GetNextAction().GetStoredAction() == null);
            controllable.SelectVectorAction(player, IntVector2.Right);

            var nextAction = acting.GetNextAction();
            Assert.AreEqual(IntVector2.Right, nextAction.direction);
            Assert.AreSame(controllable.defaultVectorAction, nextAction.GetStoredAction());

            // acting.Reset();
            // Assert.That(acting.GetNextAction().GetStoredAction() == null);
        } 
    }
}