using System.Collections.Generic;
using System.Linq;
using Hopper.Utils;
using Hopper.Core.ActingNS;
using Hopper.Core.Components.Basic;

namespace Hopper.Core
{
    public class FactionBitCache : Dictionary<Faction, HashSet<Entity>>
    {
        public void RemoveForAllBits(Faction bits, Entity entity)
        {
            foreach (var key in bits.GetSetBits())
            {
                if (TryGetValue(key, out var factionCache))
                {
                    factionCache.Remove(entity);
                }
            }
        }

        public void AddForAllBits(Faction bits, Entity entity)
        {
            foreach (var key in bits.GetSetBits())
            {
                if (TryGetValue(key, out var factionCache))
                {
                    factionCache.Add(entity);
                }
            }
        }

        /// <summary>
        /// Caches the query result of a single faction (a single flag).
        /// Simply returns the cached set. 
        /// </summary>
        public HashSet<Entity> CacheSingle(Faction flag)
        {
            if (TryGetValue(flag, out var result))
            {
                return result;
            }
            
            result = Registry.Global.RuntimeEntities.map.Values
                .Where(entity => entity.TryCheckFaction(flag))
                .ToHashSet();

            Add(flag, result);
            
            return result;
        }
    }

    /// <summary>
    /// Optimizes and caches queries by faction.
    /// When a query is first issued, it is cached so that all subsequent such queries
    /// just return the cached result.
    /// </summary>
    public struct FactionQuery
    {
        /// <summary>
        /// Stores cached query results for separate factions.
        /// </summary>
        public FactionBitCache _cache;

        public void Init()
        {
            _cache = new FactionBitCache();
        }

        /// <summary>
        /// Gets the entities of ANY given faction flags.
        /// </summary>
        public IEnumerable<Entity> Get(Faction faction)
        {
            // Empty faction matches no results
            if (faction == 0)
            {
                return Enumerable.Empty<Entity>();
            }

            // Split the faction by bits.
            var individualBits = faction.GetSetBits();

            // A single result has simpler logic
            if (individualBits.First() == faction)
            {
                return _cache.CacheSingle(faction);
            }

            // There is two ways of merging the hash sets.
            // The first one is to just do an union.
            // The other one is to go through them in order and having checked if
            // the entities in latter ones not be in any of the previous ones.
            // The selling point of the latter is that it does lazy evaluation,
            // but it's a bit slower?
            
            var result = new HashSet<Entity>();
            foreach (var flag in individualBits)
            {
                result.UnionWith(_cache.CacheSingle(flag));
            }
            return result;

            // return GetEntitiesOfAllGivenFactions(individualBits);
        }

        /// <summary>
        /// Gets the entities of ALL given flags (as an array).
        /// </summary>
        private IEnumerable<Entity> GetEntitiesOfAllGivenFactions(Faction[] bits)
        {
            var entitySets = new HashSet<Entity>[bits.Length]; 

            for (int i = 0; i < bits.Length; i++)
            {
                entitySets[i] = _cache.CacheSingle(bits[i]);
            }

            foreach (var entity in entitySets[0])
            {
                bool toKeep = true;
                for (int checkSetIndex = 1; checkSetIndex < bits.Length; checkSetIndex++)
                {
                    if (!entitySets[checkSetIndex].Contains(entity))
                    {
                        toKeep = false;
                        break;
                    }
                }
                if (toKeep)
                {
                    yield return entity;
                }
            }
        }

        /// <summary>
        /// This function should be called every time an entity gets removed from the world.
        /// </summary>
        public void RemoveEntity(Entity entity)
        {
            if (_cache.Count == 0 || !entity.TryGetFactionComponent(out var f))
            {
                return;
            }

            _cache.RemoveForAllBits(f.faction, entity);
        }

        /// <summary>
        /// This function should be called every time an entity changes its faction.
        /// </summary>
        public void ChangeEntityFaction(Entity entity, Faction oldFaction, Faction newFaction)
        {
            // Find the non-overlapping part in the factions old faction compared to the new one.
            // This part must be removed.
            Faction overlap  = oldFaction & newFaction;
            Faction toRemove = oldFaction ^ overlap;
            Faction toAdd    = newFaction ^ overlap;

            _cache.RemoveForAllBits(toRemove, entity);
            _cache.AddForAllBits(toAdd, entity);
        }

        /// <summary>
        /// This function should be called every time an entity gets added to the world.
        /// </summary>
        public void AddEntity(Entity entity)
        {
            if (_cache.Count == 0 || !entity.TryGetFactionComponent(out var f))
            {
                return;
            }

            _cache.AddForAllBits(f.faction, entity);
        }

        public void InvalidateAll()
        {
            _cache.Clear();
        }
    }
}