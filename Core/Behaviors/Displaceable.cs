using Chains;
using System.Collections.Generic;
using Core.FS;
using Vector;

namespace Core.Behaviors
{
    public class Displaceable : IBehavior
    {
        static Displaceable()
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

        Chain<Event> chain_checkDisplaced;
        Chain<Event> chain_beDisplaced;

        public Displaceable(Entity entity)
        {
            chain_checkDisplaced = (Chain<Event>)entity.m_chains["displaced:check"];
            chain_beDisplaced = (Chain<Event>)entity.m_chains["displaced:do"];
        }

        public bool Activate(Entity actor, Action action, ActivationParams pars = null)
        {
            var ev = new Event
            {
                actor = actor,
                action = action,
                move = ((Params)pars).move
            };
            chain_checkDisplaced.Pass(ev);

            if (!ev.propagate)
                return false;

            chain_beDisplaced.Pass(ev);
            return true;
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

        static BehaviorFactory<Displaceable> CreateFactory()
        {
            var fact = new BehaviorFactory<Displaceable>();

            var check = fact.AddTemplate<Event>("displaced:check");
            var convertFromMove = new EvHandler<Event>(ConvertFromMove, PRIORITY_RANKS.HIGH);
            check.AddHandler(convertFromMove);

            var _do = fact.AddTemplate<Event>("displaced:do");
            var displaceHandler = new EvHandler<Event>(Displace);
            var addEventHandler = new EvHandler<Event>(Utils.AddHistoryEvent(History.EventCode.displaced_do));
            _do.AddHandler(displaceHandler);
            _do.AddHandler(addEventHandler);

            return fact;
        }

        public static BehaviorFactory<Displaceable> s_factory = CreateFactory();
    }
}