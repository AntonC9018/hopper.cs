namespace Hopper.Core
{
    public static partial class Movs
    {
        public static Transform GetClosestPlayer(this Transform transform)
        {
            float minDist = 0;
            Transform closestPlayerTransform = null;

            // TODO: optimize such queries
            foreach (var entity in Registry.Global._entities.map.Values)
            if (entity.TryGetFaction(out var f) 
                && f.faction.HasFlag(Faction.Flags.Player)
                && entity.TryGetTransform(out var playerTransform))
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