using Hopper.Core;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Stat.Basic;
using Hopper.Core.Targeting;
using Hopper.Utils.Vector;
using NUnit.Framework;

namespace Hopper.Tests
{
    public class BufferedAtk
    {
        private World world;
        private Entity entity;
        private Wall wall;

        public BufferedAtk()
        {
            entity = new Entity();

            // this is necessary since whether the entity is attackable or not
            // (more precisely, the entities attackness)
            // matters in whether the other targets will be ignored.
            var attackable = Attackable.DefaultPreset.Instantiate(entity);
            entity.Behaviors.Add(typeof(Attackable), attackable);

            wall = new Wall();
        }

        [SetUp]
        public void Setup()
        {
            world = new World(3, 3);
            entity.Init(IntVector2.Zero, IntVector2.Zero, world);
            wall.Init(IntVector2.Zero, IntVector2.Zero, world);
        }

        [Test]
        public void SimplePattern()
        {
            /*
                E - -
                ^ - -   Expecting to get the entity as the target
                - - -
            */
            var pattern = new Pattern(
                new Piece
                {
                    dir = IntVector2.Right,
                    pos = IntVector2.Right,
                    reach = null
                }
            );
            var targetProvider = TargetProvider.CreateAtk(pattern, Handlers.DefaultAtkChain);

            entity.Pos = new IntVector2(0, 0);
            entity.ResetInGrid();

            var dummy = new Dummy(new IntVector2(0, 1), world);
            var queriedDirection = IntVector2.Up;

            var targets = targetProvider.GetTargets(dummy, queriedDirection);

            Assert.AreSame(entity, targets[0].entity);
        }

        [Test]
        public void SpearPattern()
        {
            /*
                E - -
             1  - - -   Expecting to get the entity as the target
                ^ - -

                E - -
             2  B - -   Expecting to get an empty list, since the reach is not set to null 
                ^ - -
            */
            var pattern = new Pattern(
                new Piece
                {
                    dir = IntVector2.Right,
                    pos = IntVector2.Right,
                    reach = null
                },
                new Piece
                {
                    dir = IntVector2.Right,
                    pos = IntVector2.Right * 2,
                    reach = new int[0]
                }
            );
            var targetProvider = TargetProvider.CreateAtk(pattern, Handlers.DefaultAtkChain);

            entity.Pos = new IntVector2(0, 0);
            entity.ResetInGrid();

            // 1
            var dummy = new Dummy(new IntVector2(0, 2), world);
            var queriedDirection = IntVector2.Up;

            var targets = targetProvider.GetTargets(dummy, queriedDirection);

            Assert.AreSame(entity, targets[0].entity);

            // 2
            wall.Pos = new IntVector2(0, 1);
            wall.ResetInGrid();

            targets = targetProvider.GetTargets(dummy, queriedDirection);

            Assert.AreEqual(0, targets.Count);
        }

        [Test]
        public void IfCloseTest()
        {
            /*
                For this test, assume the entity is attackable only if close

                E - -
             1  ^ - -   Expecting to get the entity as the target
                - - -

                E - -
             2  - - -   Expecting to NOT get the entity as the target
                ^ - -
            */
            var pattern = new Pattern(
                new Piece
                {
                    dir = IntVector2.Right,
                    pos = IntVector2.Right,
                    reach = null
                },
                new Piece
                {
                    dir = IntVector2.Right,
                    pos = IntVector2.Right * 2,
                    reach = null
                }
            );
            var targetProvider = TargetProvider.CreateAtk(pattern, Handlers.DefaultAtkChain);

            // say now the entity is only attackable while close
            entity.Behaviors.Get<Attackable>().m_attackness |= Attackness.IF_NEXT_TO;

            entity.Pos = new IntVector2(0, 0);
            entity.ResetInGrid();

            var queriedDirection = IntVector2.Up;

            // 1
            var dummy = new Dummy(new IntVector2(0, 1), world);
            var targets = targetProvider.GetTargets(dummy, queriedDirection);
            Assert.AreSame(entity, targets[0].entity);

            // 2
            dummy = new Dummy(new IntVector2(0, 2), world);
            targets = targetProvider.GetTargets(dummy, queriedDirection);
            Assert.AreEqual(0, targets.Count);

            // do a clean-up
            entity.Behaviors.Get<Attackable>().m_attackness ^= Attackness.IF_NEXT_TO;
        }
    }
}