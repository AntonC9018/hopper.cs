using System.Collections.Generic;
using Vector;
using Chains;

namespace Core.Weapon
{
    public class Piece
    {
        public IntVector2 pos;
        public IntVector2 dir;
        // null is no checking required
        // empty list to check all previous indeces
        // list of indices to check the specified indeces
        public List<int> reach;
        public Piece Rotate(double angle)
        {
            return new Piece
            {
                pos = pos.Rotate(angle),
                dir = dir.Rotate(angle),
                reach = reach
            };
        }
    }
    public abstract class Target
    {
        public int index;
        public Piece initialPiece;
        public Entity entity;
        public IntVector2 direction;

        public virtual void CalculateCondition(CommonEvent ev)
        { }
    }
    public class AtkTarget : Target
    {
        public AtkCondition atkCondition;

        public override void CalculateCondition(CommonEvent ev)
        {
            if (ev.GetType() != typeof(Attacking.Event))
            {
                throw new System.Exception("Expected event type to be Attacking.Event");
            }
            if (entity != null)
            {
                var attackable = entity.beh_Attackable;

                atkCondition =
                    attackable == null
                        ? AtkCondition.NEVER
                        // TODO: this requires action with the attack already set
                        : attackable.GetAttackableness();
            }
            else
            {
                atkCondition = AtkCondition.NEVER;
            }
        }
    }



    // static List<Piece> defaultPattern = new List<Piece>
    // {
    //     new Piece
    //     {
    //         pos = new IntVector2(1, 0),
    //         dir = new IntVector2(1, 0),
    //         reach = null
    //     }
    // };

    public class Weapon<T> where T : Target, new()
    {
        public class Event : CommonEvent
        {
            public List<T> targets;
        }
        List<Piece> pattern;
        Chain<Event> chain;
        System.Func<Event, bool> check;
        Layer attackedLayer = Layer.REAL | Layer.MISC | Layer.WALL;

        public Weapon(List<Piece> pattern, Chain<Event> chain, System.Func<Event, bool> check)
        {
            this.pattern = pattern;
            this.chain = chain;
            this.check = check;
        }

        public List<T> GetTargets(CommonEvent commonEvent)
        {
            var targets = new List<T>();
            double angle = IntVector2.Right.AngleTo(commonEvent.action.direction);

            for (int i = 0; i < this.pattern.Count; i++)
            {
                var piece = this.pattern[i].Rotate(angle);
                System.Console.WriteLine(piece.pos);

                var pos = commonEvent.actor.m_pos + piece.pos;
                var entity = commonEvent.actor.m_world.m_grid
                    .GetCellAt(pos)
                    .GetEntityFromLayer(attackedLayer);

                var target = new T
                {
                    direction = piece.dir,
                    entity = entity,
                    index = i,
                    initialPiece = this.pattern[i]
                };
                target.CalculateCondition(commonEvent);
                targets.Add(target);
            }

            var ev = new Event
            {
                targets = targets,
                actor = commonEvent.actor,
                action = commonEvent.action
            };

            chain.Pass(ev, check);

            return ev.targets;
        }
    }
}