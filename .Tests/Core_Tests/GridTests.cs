using System.Linq;
using Hopper.Core;
using Hopper.Utils.Vector;
using NUnit.Framework;

namespace Hopper.Tests
{
    public class GridTests
    {
        public EntityFactory wallFactory;
        public EntityFactory entityFactory;
        public EntityFactory directionalBlockFactory;

        public GridTests()
        {
            InitScript.Init();

            wallFactory = new EntityFactory();
            Transform.AddTo(wallFactory, Layer.WALL);

            entityFactory = new EntityFactory();
            Transform.AddTo(entityFactory, Layer.REAL);

            directionalBlockFactory = new EntityFactory();
            Directed.AddTo(directionalBlockFactory);
            Transform.AddTo(directionalBlockFactory, Layer.WALL);
        }

        public GridManager Grid => World.Global.grid;

        [SetUp]
        public void Setup()
        {
            World.Global = new World(3, 3);
        }

        [Test]
        public void OutOfBoundTest()
        {
            Assert.False(Grid.IsInBounds(new IntVector2(-1, 0)));
            Assert.True(Grid.IsOutOfBounds(new IntVector2(-1, 0)));
        }

        [Test]
        public void PlacesEntitiesCorrectly()
        {
            var entity = World.Global.SpawnEntity(entityFactory, new IntVector2(0, 0));
            var transform = entity.GetTransform();
            var cell = Grid.GetCellAt(new IntVector2(0, 0));
            Assert.AreSame(transform, cell.First());
            Assert.AreSame(
                transform,
                Grid.GetTransformFromLayer(transform.position, new IntVector2(0, 0), transform.layer));
            Assert.True(Grid.HasUndirectedTransformAt(transform.position, transform.layer));
            Assert.False(transform.GetAllButSelfFromLayer(transform.layer).Any());
        }

        [Test]
        public void DeletesEntitiesCorrectly()
        {
            var entity = World.Global.SpawnEntity(entityFactory, new IntVector2(0, 0));
            var transform = entity.GetTransform();
            Assert.True(Grid.HasUndirectedTransformAt(transform.position, transform.layer));
            
            transform.RemoveFromGrid();
            Assert.False(Grid.HasUndirectedTransformAt(transform.position, transform.layer));

            transform.ResetInGrid();
            Assert.True(Grid.HasUndirectedTransformAt(transform.position, transform.layer));

            Assert.True(Grid.TryRemove(transform));
            Assert.False(Grid.HasUndirectedTransformAt(transform.position, transform.layer));
        }

        [Test]
        public void CellMovementListenersWorkCorrectly()
        {
            var entity = World.Global.SpawnEntity(entityFactory, new IntVector2(0, 0));
            var transform = entity.GetTransform();

            Transform otherTransform = null;
            transform.SubsribeToEnterEvent(ctx => otherTransform = ctx.transform); 
            transform.SubsribeToLeaveEvent(ctx => otherTransform = ctx.transform); 

            transform.RemoveFromGrid();
            Assert.AreSame(transform, otherTransform);
            
            otherTransform = null;

            transform.ResetInGrid();
            Assert.AreSame(transform, otherTransform);

            otherTransform = null;

            Grid.ResetCellTriggers();

            transform.RemoveFromGrid();
            Assert.Null(otherTransform);

            transform.ResetInGrid();
            Assert.Null(otherTransform);
        }

        [Test]
        public void CellMovementPermanentListenersWorkCorrectly()
        {
            var entity = World.Global.SpawnEntity(entityFactory, new IntVector2(0, 0));
            var transform = entity.GetTransform();

            Transform otherTransform = null;
            bool keep = true;
            transform.SubsribeToPermanentEnterEvent(ctx => { otherTransform = ctx.transform; return keep; }); 
            transform.SubsribeToPermanentLeaveEvent(ctx => { otherTransform = ctx.transform; return keep; }); 

            transform.RemoveFromGrid();
            Assert.AreSame(transform, otherTransform);
            
            otherTransform = null;

            transform.ResetInGrid();
            Assert.AreSame(transform, otherTransform);

            keep = false;

            transform.RemoveFromGrid();
            transform.ResetInGrid();

            otherTransform = null;

            transform.RemoveFromGrid();
            Assert.Null(otherTransform);

            transform.ResetInGrid();
            Assert.Null(otherTransform);
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

            Assert.False(Grid.HasBlockAt(queriedPosition, queriedDirection, Layer.REAL)); // no entity place there yet

            var entity = World.Global.SpawnEntity(entityFactory, queriedPosition);
            var transform = entity.GetTransform();

            Assert.True(Grid.HasBlockAt(queriedPosition, queriedDirection, Layer.REAL));
            // wall has the wall layer type, which we disregard for this test (no walls)
            Assert.False(Grid.HasBlockAt(queriedPosition, queriedDirection, Layer.WALL));
            Assert.True(Grid.HasBlockAt(queriedPosition, queriedDirection, Layer.WALL | Layer.REAL));
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

            var pos = new IntVector2(0, 0);
            var dir = IntVector2.Up;

            // The orientation actually controls on which side of the block it is
            var directionalBlock = World.Global.SpawnEntity(directionalBlockFactory, pos, IntVector2.Down);
            var directionalBlockTransform = directionalBlock.GetTransform();

            // 1
            // I set the layer of the directional block to `Wall`, see above
            Assert.True(Grid.HasBlockAt(pos, dir, Layer.WALL));
            // Here we ignore the wall Layer
            Assert.False(Grid.HasBlockAt(pos, dir, Layer.REAL));

            // 2
            dir = IntVector2.Left;
            Assert.False(Grid.HasBlockAt(pos, dir, Layer.WALL));

            // 3
            var entity = World.Global.SpawnEntity(entityFactory, new IntVector2(0, 0));
            var transform = entity.GetTransform();

            // If we still ignore the entity layer, nothing happens
            Assert.False(Grid.HasBlockAt(pos, dir, Layer.WALL));
            // Now we that take into account both of the layers, it becomes blocked
            Assert.True(Grid.HasBlockAt(pos, dir, Layer.WALL | Layer.REAL));
            // Same if we consider just the entity
            Assert.True(Grid.HasBlockAt(pos, dir, Layer.REAL));

            // 4 Setup
            transform.RemoveFromGrid();
            directionalBlockTransform.RemoveFromGrid();

            directionalBlockTransform.position += IntVector2.Down;
            directionalBlockTransform.orientation = IntVector2.Up;

            directionalBlockTransform.ResetInGrid();

            // 4
            dir = IntVector2.Up;
            Assert.True(Grid.HasBlockAt(pos, dir, Layer.WALL));

            dir = IntVector2.Down;
            Assert.False(Grid.HasBlockAt(pos, dir, Layer.WALL));

            pos += IntVector2.Down;
            dir = IntVector2.Up;
            Assert.False(Grid.HasBlockAt(pos, dir, Layer.WALL));
            
            dir = IntVector2.Down;
            Assert.True(Grid.HasBlockAt(pos, dir, Layer.WALL));
        }

    }
}