using System.Linq;
using Hopper.Core;
using Hopper.Core.WorldNS;
using Hopper.Utils.Chains;
using Hopper.Utils.Vector;
using NUnit.Framework;

namespace Hopper.Tests
{
    public class WorldTests
    {
        public WorldTests()
        {
            InitScript.Init();
        }

        [SetUp]
        public void Setup()
        {
            World.Global = new World(1, 1);
        }


        // This test is kind of trash, because if new phases get added, it would break.
        [Test]
        public void CorrectPhaseOrderingTest()
        {
            var enumerator = World.Global.LoopCoroutine().GetEnumerator();
            enumerator.MoveNext(); Assert.AreEqual(enumerator.Current, Phase.Calculate_Actions);

            enumerator.MoveNext(); Assert.AreEqual(enumerator.Current, Phase.Player_Act);
            enumerator.MoveNext(); Assert.AreEqual(enumerator.Current, Phase.Entity_Act);
            enumerator.MoveNext(); Assert.AreEqual(enumerator.Current, Phase.Trap_Act);
            enumerator.MoveNext(); Assert.AreEqual(enumerator.Current, Phase.Projectile_Act);

            enumerator.MoveNext(); Assert.AreEqual(enumerator.Current, Phase.Ticking);
            enumerator.MoveNext(); Assert.AreEqual(enumerator.Current, Phase.FilterDead);

            Assert.False(enumerator.MoveNext());
            Assert.AreEqual(World.Global.State.currentPhase, Phase.Done);
        }

        [Test]
        public void GlobalChainsTest()
        {
            int callCount = 0;
            var handler = new Handler<int>(10, loopCount => callCount++);
            
            World.StartLoopPath.Get().Add(handler);
            World.Global.Loop();
            Assert.AreEqual(callCount, 1);

            World.StartLoopPath.Get().Remove(handler);
            World.Global.Loop();
            Assert.AreEqual(callCount, 1);

            World.EndLoopPath.Get().Add(handler);
            World.Global.Loop();
            Assert.AreEqual(callCount, 2);
        }

    }
}