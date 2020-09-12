namespace Core
{
    public static partial class Movs
    {
        public class Relativeness
        {
            public bool gx, gy, lx, ly;
        }
        public static Relativeness CalculateRelativeness(Entity actor, Entity player)
        {
            return new Relativeness
            {
                gx = player.Pos.x > actor.Pos.x,
                gy = player.Pos.y > actor.Pos.y,
                lx = actor.Pos.x > player.Pos.x,
                ly = actor.Pos.y > player.Pos.y
            };
        }
    }
}