namespace Hopper.Core
{
    public static partial class Movs
    {
        public static Entity GetClosestPlayer(this Transform transform)
        {
            float minDist = 0;
            Entity closestPlayerTransform = null;
            foreach (var player in World.Global.State.Players)
            {
                float curDist = (transform.position - player.GetTransform().position).SqMag;

                if (closestPlayerTransform == null || curDist < minDist)
                {
                    minDist = curDist;
                    closestPlayerTransform = player;
                }
            }
            return closestPlayerTransform;
        }

        public static bool TryGetClosestPlayer(this Transform transform, out Entity player)
        {
            player = GetClosestPlayer(transform);
            return player != null;
        }

        // TODO: this will be redone
        public static bool TryGetClosestPlayerTransform(this Transform transform, out Transform playerTransform)
        {
            var player = GetClosestPlayer(transform);
            if (player != null)
            {
                playerTransform = player.GetTransform();
                return true;
            }
            playerTransform = null;
            return false;
        }
    }
}