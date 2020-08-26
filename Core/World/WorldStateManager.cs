using System.Collections.Generic;

namespace Core
{
    public class WorldStateManager
    {
        List<Entity>[] entities
            = new List<Entity>[System.Enum.GetNames(typeof(Layer)).Length];

        // note that players never get into the lists from above
        public List<Entity> m_players = new List<Entity>();

        // Subscribe to this event only from the entity
        // This easily prevents memory leaks. How?
        // The handlers added to this event are strong pointers.
        // This event is replicated on the entity, to control this.
        public event System.Action EndOfLoopEvent;

        public int m_phase = 0;

        public WorldStateManager()
        {
            for (int i = 0; i < entities.Length; i++)
            {
                entities[i] = new List<Entity>();
            }
        }

        public void AddEntity(Entity entity)
        {
            entities[entity.m_layer.ToIndex()].Add(entity);
        }

        public int AddPlayer(Entity player)
        {
            m_players.Add(player);
            return m_players.Count - 1;
        }

        void CalculateNextAction(Entity entity)
        {
            entity.beh_Acting?.CalculateNextAction();
        }

        void Activate(Entity entity)
        {
            if (entity.b_isDead) return;
            var acting = entity.beh_Acting;
            if (acting != null && !acting.b_didAction)
            {
                // I've overloaded the Activate method here so that it is not as clunky
                acting.Activate();
            }
        }

        public void Loop()
        {
            foreach (var player in m_players)
            {
                Activate(player);
            }

            // Ideally, the acting decorators should be at one location in memory
            for (int i = 0; i < entities.Length; i++)
                foreach (var e in entities[i])
                    CalculateNextAction(e);

            for (int i = 0; i < entities.Length; i++)
            {
                m_phase = i;
                foreach (var e in entities[i])
                    Activate(e);
            }

            EndOfLoopEvent?.Invoke();
        }

    }
}