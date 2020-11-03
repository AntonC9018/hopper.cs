using Core;
using Core.Behaviors;

namespace Test
{
    public class TestBoss : Entity
    {
        private static Step[] Steps = new[]
        {
            new Step
            {
                action = Laser.LaserShootAction,
                movs = Movs.Basic
            },
            new Step
            {
                repeat = 3
            }
        };
        public static readonly EntityFactory<TestBoss> Factory = new EntityFactory<TestBoss>()
            .AddBehavior<Acting>(new Acting.Config(Algos.EnemyAlgo))
            .AddBehavior<Sequential>(new Sequential.Config(Steps));
    }
}