using System.Numerics;
using Chains;

namespace Core
{
    static partial class Algos
    {
        static bool AskMove(Acting.ActingEvent ev)
        {
            Entity thing = ev.actor.m_world.m_grid
                .GetCellAt(ev.actor.m_pos + ev.action.direction) // change to action.direction
                .GetEntityFromLayer(Layer.REAL);

            if (thing == null)
                return false;

            var acting = thing.beh_Acting;

            if (acting == null || acting.b_didAction || acting.b_doingAction)
                return false;

            acting.Activate();

            return true;
        }

        static bool Iterate(Acting.ActingEvent ev)
        {
            bool success = ev.action.Do(ev.actor);

            if (!success)
            {
                bool otherEntitySuccess = AskMove(ev);

                if (otherEntitySuccess)
                    return Iterate(ev);
            }

            return success;
        }

        public static void EnemyAlgo(EventBase _ev)
        {
            var ev = (Acting.ActingEvent)_ev;

            var dirs = ev.actor.beh_Sequenced.GetMovs();

            foreach (var dir in dirs)
            {
                // var action = ev.action.Copy();
                ev.action.direction = dir;
                if (Iterate(ev))
                {
                    ev.success = true;
                    return;
                }
            }

            ev.success = false;
        }

    }
}