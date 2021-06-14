using System.Collections.Generic;
using System.Linq;
using Hopper.Core.ActingNS;
using Hopper.Core.Components.Basic;

namespace Hopper.Core
{
    /// <summary>
    /// Optimizes and caches queries by faction.
    /// When a query is first issued, it is cached so that all subsequent such queries
    /// just return the cached result.
    /// </summary>
    public struct FactionQuery
    {
        public Dictionary<Faction, Entity[]> _cache;

        public void Init()
        {
            _cache = new Dictionary<Faction, Entity[]>();
        }

        public Entity[] Get(Faction faction)
        {
            if (_cache.TryGetValue(faction, out var result))
            {
                return result;
            }

            result = Registry.Global.RuntimeEntities.map.Values
                .Where(entity => entity.TryCheckFaction(faction, out bool res) && res)
                .ToArray();
            
            _cache.Add(faction, result);

            return result;
        }

        private static IEnumerable<int> BitCombos(int bits)
        {
            int current = (~bits + 1) & bits;
            while (current != 0)
            {
                yield return current;
                current = (current - bits) & bits;
            }
        }

        /// <summary>
        /// This function should be called every time an entity gets added into the world.
        /// </summary>
        public void Invalidate(Entity entity)
        {
            if (_cache.Count == 0 || !entity.TryGetFactionComponent(out var f))
            {
                return;
            }

            // If the faction covers all possible cached arrays, remove all
            if (f.faction == Faction.Any)
            {
                _cache.Clear();
                return;
            }

            // Invalidate all cached groups.
            // TODO: It might be more efficient to iterate through keys of the dictionary instead.
            foreach (var factionCombo in BitCombos((int) f.faction))
            {
                _cache.Remove((Faction) factionCombo);
            }
        }
    }
}