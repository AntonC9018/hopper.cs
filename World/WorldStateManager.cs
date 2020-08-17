using System.Collections.Generic;

namespace Core
{
    public class WorldStateManager
    {
        List<Entity>[] entities
            = new List<Entity>[System.Enum.GetNames(typeof(Layer)).Length];

        // note that players never get into the lists from above
        public List<Entity> m_players = new List<Entity>();

        public int m_phase = 0;

        public WorldStateManager()
        {
            for (int i = 0; i < entities.Length; i++)
            {
                entities[i] = new List<Entity>();
            }
        }

        public int AddPlayer(Entity player)
        {
            m_players.Add(player);
            return m_players.Count - 1;
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

            for (int i = 0; i < entities.Length; i++)
            {
                m_phase = i;
                foreach (var e in entities[i])
                {
                    Activate(e);
                }
            }
        }



    }
}