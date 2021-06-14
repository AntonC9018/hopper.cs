using Hopper.Core.WorldNS;

namespace Hopper.Core.ActingNS
{
    public static partial class Movs
    {
        public static Transform GetClosestPlayer(this Transform transform)
        {
            float minDist = 0;
            Transform closestPlayerTransform = null;

            foreach (var player in Registry.Global.Queries.Faction.Get(Faction.Player))
            if (player.TryGetTransform(out var playerTransform))
            {
                float curDist = (transform.position - playerTransform.position).SqMag;

                if (closestPlayerTransform == null || curDist < minDist)
                {
                    minDist = curDist;
                    closestPlayerTransform = playerTransform;
                }
            }
            return closestPlayerTransform;
        }

        public static bool TryGetClosestPlayer(this Transform transform, out Entity player)
        {
            player = GetClosestPlayer(transform)?.entity;
            return player != null;
        }

        // TODO: this will be redone
        public static bool TryGetClosestPlayerTransform(this Transform transform, out Transform playerTransform)
        {
            var player = GetClosestPlayer(transform);
            if (player != null)
            {
                playerTransform = player;
                return true;
            }
            playerTransform = null;
            return false;
        }
    }
}