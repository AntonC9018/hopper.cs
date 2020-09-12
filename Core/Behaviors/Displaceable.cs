using Chains;
using System.Collections.Generic;
using Core.FS;
using Vector;

namespace Core.Behaviors
{
    public class Displaceable : Behavior
    {
        static void SetupStats()
        {
            var baseDir = StatManager.DefaultFS.BaseDir;

            var move = new Move
            {
                power = 1,
                through = 0
            };

            baseDir.AddFile("move", move);
        }

        public class Move : StatFile
        {
            public int power = 1;
            public int through = 0;
        }

        public class Event : CommonEvent
        {
            public Entity entity;
            public Move move;
            public IntVector2 newPos;
        }

        public class Params : ActivationParams
        {
            public Move move;
        }
        public static string s_checkChainName = "displaced:check";
        public static string s_doChainName = "displaced:do";

        public bool Activate(Action action, Params pars)
        {
            var ev = new Event
            {
                actor = m_entity,
                action = action,
                move = pars.move
            };
            return CheckDoCycle<Event>(ev, s_checkChainName, s_doChainName);
        }

        static void ConvertFromMove(Event ev)
        {
            int i = 1;

            do
            {
                var cell = ev.actor.GetCellRelative(ev.action.direction * i);

                if (cell == null || cell.GetEntityFromLayer(Layer.BLOCK) != null)
                    break;
                i++;
            } while (i < ev.move.power);
            i--;

            ev.newPos = ev.actor.GetPosRelative(ev.action.direction * i);
        }

        static void Displace(Event ev)
        {
            ev.actor.ResetPosInGrid(ev.newPos);
        }

        public static ChainPaths<Displaceable, Event> Check;
        public static ChainPaths<Displaceable, Event> Do;

        static Displaceable()
        {
            var builder = new ChainTemplateBuilder();

            var check = builder.AddTemplate<Event>(s_checkChainName);
            Check = new ChainPaths<Displaceable, Event>(s_checkChainName);
            check.AddHandler(ConvertFromMove, PRIORITY_RANKS.HIGH);

            var _do = builder.AddTemplate<Event>(s_doChainName);
            Do = new ChainPaths<Displaceable, Event>(s_doChainName);
            _do.AddHandler(Displace);
            _do.AddHandler(Utils.AddHistoryEvent(History.EventCode.displaced_do));

            BehaviorFactory<Displaceable>.s_builder = builder;

            SetupStats();
        }

    }
}