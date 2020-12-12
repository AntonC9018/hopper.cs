using Hopper.Core;
using Hopper.Utils.Vector;
using NUnit.Framework;

namespace Hopper.Tests
{
    public class Grid_Test
    {
        public GridManager grid;
        public Entity entity; // has the real layer
        public Wall wall; // has the wall layer

        public class DirectionalBlock : Wall
        {
            public override Layer Layer => Layer.WALL;
            public override bool IsDirected => true;
        }

        public DirectionalBlock directionalBlock;

        public Grid_Test()
        {
            entity = new Entity();
            wall = new Wall();
            directionalBlock = new DirectionalBlock();
        }

        [SetUp]
        public void Setup()
        {
            grid = new GridManager(3, 3);
        }

        [Test]
        public void GettingCellOutOfBounds_ReturnsNull()
        {
            Assert.IsNull(grid.GetCellAt(new IntVector2(-1, 0)));
        }

        [Test]
        public void PlacesEntitiesCorrectly()
        {
            entity.Pos = new IntVector2(0, 0);
            grid.Reset(entity);
            var cell = grid.GetCellAt(new IntVector2(0, 0));
            Assert.AreSame(entity, cell.GetFirstEntity());
        }

        [Test]
        public void DeletesEntitiesCorrectly()
        {
            entity.Pos = new IntVector2(0, 0);
            grid.Reset(entity);
            grid.Remove(entity);

            var cell = grid.GetCellAt(new IntVector2(0, 0));

            Assert.AreEqual(0, cell.m_entities.Count);

            Assert.Throws<Hopper.Utils.Exception>(() => grid.Remove(entity));
        }

        [Test]
        public void EnterListenerWorksCorrectly()
        {
            var cell = grid.GetCellAt(new IntVector2(0, 0));

            Entity enteredEntity = null;
            cell.EnterEvent += e => enteredEntity = e;

            entity.Pos = new IntVector2(0, 0);
            grid.Reset(entity);

            Assert.AreSame(enteredEntity, entity);
        }

        [Test]
        public void LeaveListenerWorksCorrectly()
        {
            var cell = grid.GetCellAt(new IntVector2(0, 0));

            Entity leftEntity = null;
            cell.LeaveEvent += e => leftEntity = e;

            entity.Pos = new IntVector2(0, 0);
            grid.Reset(entity);
            grid.Remove(entity);

            Assert.AreSame(leftEntity, entity);
        }

        [Test]
        public void WayIsConsideredBlockedCorrectly_ForUndirectedEntities()
        {

            /*
                E - -      E is where the entity stands
                ^ - -      The caret represents the queried direction
                - - -      Expected the way to be blocked (assuming queried the right layer)
            */

            var queriedPosition = new IntVector2(0, 0);
            var queriedDirection = new IntVector2(-1, 0); // equivalently, IntVector2.Up
            var cell = grid.GetCellAt(queriedPosition);

            Assert.False(cell.HasBlock(queriedDirection, entity.Layer)); // no entity place there yet

            entity.Pos = queriedPosition;
            grid.Reset(entity);

            Assert.True(cell.HasBlock(queriedDirection, entity.Layer));
            // wall has the wall layer type, which we disregard for this test (no walls)
            Assert.False(cell.HasBlock(queriedDirection, wall.Layer));
            Assert.True(cell.HasBlock(queriedDirection, entity.Layer | wall.Layer));
        }

        [Test]
        public void WayIsConsideredBlockedCorrectly_ForDirectedEntities()
        {

            /*   _______
                |       |
                |       |   ##### is the directed block, with orientation = Down
             1  |_#####_|   Expecting in this case being blocked only from the bottom
                |   ^   |
                |   |   |
                |_______|

                 _______ _______
                |       |       |
             2  |       | <---  |   In this case, expect it to not be blocked.
                |_#####_|_______|

                 _______ _______
                |       |       |
              3 | Entty | <---  |   In this case, though, it would be considered blocked.
                |_#####_|_______|   

                 _______
                |       |
                |       |   ##### is the directed block, with orientation = Up
             4  |_______|   However, in this case the block before the queried one gets checked
                | ##### |   Expecting in this case being blocked only from the bottom
                |   ^   |
                |___|___|
               
            */

            var queriedPosition = new IntVector2(0, 0);
            var queriedDirection = IntVector2.Up;
            var cell = grid.GetCellAt(queriedPosition);

            directionalBlock.Pos = queriedPosition;
            // The orientation actually controls on which side of the block it is
            directionalBlock.Orientation = IntVector2.Down;

            grid.Reset(directionalBlock);

            // 1
            // I set the layer of the directional block to `Wall`, see above
            Assert.True(cell.HasBlock(queriedDirection, directionalBlock.Layer));
            // Here we ignore the wall Layer
            Assert.False(cell.HasBlock(queriedDirection, entity.Layer));

            // 2
            queriedDirection = IntVector2.Left;
            Assert.False(cell.HasBlock(queriedDirection, directionalBlock.Layer));

            // 3
            entity.Pos = queriedPosition;
            grid.Reset(entity);

            // If we still ignore the entity layer, nothing happens
            Assert.False(cell.HasBlock(queriedDirection, directionalBlock.Layer));
            // Now we that take into account both of the layers, it becomes blocked
            Assert.True(cell.HasBlock(queriedDirection, directionalBlock.Layer | entity.Layer));
            // Same if we consider just the entity
            Assert.True(cell.HasBlock(queriedDirection, entity.Layer));

            // 4 Setup
            grid.Remove(entity);
            grid.Remove(directionalBlock);

            queriedDirection = IntVector2.Up;

            directionalBlock.Pos += IntVector2.Down;
            directionalBlock.Orientation = IntVector2.Up;

            grid.Reset(directionalBlock);

            // 4
            Assert.True(cell.HasBlock(queriedDirection, directionalBlock.Layer));
        }

    }
}