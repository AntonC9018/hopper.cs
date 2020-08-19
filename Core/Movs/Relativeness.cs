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
                gx = player.m_pos.X > actor.m_pos.X,
                gy = player.m_pos.Y > actor.m_pos.Y,
                lx = actor.m_pos.X > player.m_pos.X,
                ly = actor.m_pos.Y > player.m_pos.Y
            };
        }
    }
}