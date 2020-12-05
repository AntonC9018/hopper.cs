namespace Hopper.Core
{
    public static partial class Movs
    {
        public static Entity GetClosestPlayer(this Entity entity)
        {
            float minDist = 0;
            Entity closestPlayer = null;
            foreach (var player in entity.World.State.Players)
            {
                float curDist = (entity.Pos - player.Pos).SqMag;

                if (closestPlayer == null || curDist < minDist)
                {
                    minDist = curDist;
                    closestPlayer = player;
                }
            }
            return closestPlayer;
        }
    }
}