using Hopper.Core;
using Hopper.Core.Behaviors;
using Hopper.Core.Behaviors.Basic;
using Hopper.Core.Stats.Basic;
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
            var attackable = new Attackable();
            attackable.GenerateChains(BehaviorFactory<Attackable>.s_builder.Templates);
            entity.Behaviors.Add(typeof(Attackable), attackable);

            wall = new Wall();
        }

        [SetUp]
        public void Setup()
        {
            world = new World(3, 3);
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
            var targetProvider = TargetProvider.CreateAtk(pattern, Handlers.GeneralChain);

            entity.Pos = new IntVector2(0, 0);
            world.Grid.Reset(entity);

            var dummy = new Dummy(new IntVector2(0, 1), world);
            var queriedDirection = IntVector2.Up;
            var attack = new Attack(); // leave everything at 0, since not actually attacking

            var targets = targetProvider.GetTargets(dummy, queriedDirection, attack);

            Assert.AreSame(entity, targets[0].entity);
        }

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
                    pos = IntVector2.Right,
                    reach = new int[0]
                }
            );
            var targetProvider = TargetProvider.CreateAtk(pattern, Handlers.GeneralChain);

            entity.Pos = new IntVector2(0, 0);
            world.Grid.Reset(entity);

            var dummy = new Dummy(new IntVector2(0, 2), world);
            var queriedDirection = IntVector2.Up;
            var attack = new Attack(); // leave everything at 0, since not actually attacking

            var targets = targetProvider.GetTargets(dummy, queriedDirection, attack);

            Assert.AreSame(entity, targets[0].entity);

            wall.Pos = new IntVector2(0, 1);
            world.Grid.Reset(wall);

            targets = targetProvider.GetTargets(dummy, queriedDirection, attack);

            Assert.AreEqual(0, targets.Count);

        }

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
            var targetProvider = TargetProvider.CreateAtk(pattern, Handlers.GeneralChain);

            // say now the entity is only attackable while close
            var handle = Attackable.Condition.ChainPath(entity.Behaviors)
                .AddHandler(ev => ev.attackness = Attackness.IF_NEXT_TO);

            entity.Pos = new IntVector2(0, 0);
            world.Grid.Reset(entity);

            var queriedDirection = IntVector2.Up;
            var attack = new Attack(); // leave everything at 0, since not actually attacking

            // 1
            var dummy = new Dummy(new IntVector2(0, 1), world);
            var targets = targetProvider.GetTargets(dummy, queriedDirection, attack);
            Assert.AreSame(entity, targets[0]);

            // 2
            dummy = new Dummy(new IntVector2(0, 2), world);
            targets = targetProvider.GetTargets(dummy, queriedDirection, attack);
            Assert.AreEqual(0, targets.Count);

            // do a clean-up
            Attackable.Condition.ChainPath(entity.Behaviors).RemoveHandler(handle);
        }
    }
}