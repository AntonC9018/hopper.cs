using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Targeting;
using Hopper.TestContent;
using Hopper.TestContent.SimpleMobs;
using Hopper.Utils;
using Hopper.Utils.Vector;

namespace Hopper.Mine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Hopper.Core.Main.Init();
            Hopper.TestContent.Main.Init();

            var wallFactory = new EntityFactory();
            Transform.AddTo(wallFactory, Layer.WALL);

            var entityFactory = new EntityFactory();
            Transform.AddTo(entityFactory, Layer.REAL);
            Attackable.AddTo(entityFactory, Attackness.ALWAYS);
            World.Global = new World(3, 3);
            
            var pattern = new PieceAttackPattern(
                new Piece(new IntVector2(1, 0), new IntVector2(1, 0), new Reach(true)),
                new Piece(new IntVector2(2, 0), new IntVector2(1, 0), new Reach(0))
            );
            var provider = new BufferedAttackTargetProvider(pattern,
                BufferedAttackTargetProvider.SingleDefaultMap, Layer.REAL, Layer.WALL);
            var entity = World.Global.SpawnEntity(entityFactory, new IntVector2(0, 0));

            AttackTargetingContext context;

            entity.GetAttackable()._attackness = Attackness.CAN_BE_ATTACKED_IF_NEXT_TO;
            context = provider.GetTargets(null, new IntVector2(0, 2), new IntVector2(0, -1));
            Assert.That(context.targetContexts.Count == 0);
        }
    }
}