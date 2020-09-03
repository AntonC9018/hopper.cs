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
            var baseDir = StatManager.s_defaultFS.BaseDir;

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

        public Displaceable(Entity entity)
        {
        }

        public bool Activate(Entity actor, Action action, ActivationParams pars = null)
        {
            var ev = new Event
            {
                actor = actor,
                action = action,
                move = ((Params)pars).move
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

            ev.newPos = ev.actor.GetRelativePos(ev.action.direction * i);
        }

        static void Displace(Event ev)
        {
            ev.actor.RemoveFromGrid();
            ev.actor.m_pos = ev.newPos;
            ev.actor.ResetInGrid();
        }

        public static ChainPath<Displaceable, Event> check_chain;
        public static ChainPath<Displaceable, Event> do_chain;

        static Displaceable()
        {
            var builder = new ChainTemplateBuilder();

            var check = builder.AddTemplate<Event>(s_checkChainName);
            check_chain = new ChainPath<Displaceable, Event>(s_checkChainName);
            check.AddHandler(ConvertFromMove, PRIORITY_RANKS.HIGH);

            var _do = builder.AddTemplate<Event>(s_doChainName);
            do_chain = new ChainPath<Displaceable, Event>(s_doChainName);
            _do.AddHandler(Displace);
            _do.AddHandler(Utils.AddHistoryEvent(History.EventCode.displaced_do));

            BehaviorFactory<Displaceable>.s_builder = builder;

            SetupStats();
        }

    }
}