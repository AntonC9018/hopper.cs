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
                gx = player.m_pos.x > actor.m_pos.x,
                gy = player.m_pos.y > actor.m_pos.y,
                lx = actor.m_pos.x > player.m_pos.x,
                ly = actor.m_pos.y > player.m_pos.y
            };
        }
    }
}