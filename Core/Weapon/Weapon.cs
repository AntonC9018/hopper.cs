using System.Collections.Generic;
using Vector;
using Chains;

namespace Core.Weapon
{
    public class Weapon
    {

        public class Piece
        {
            public Vector2 pos;
            public Vector2 dir;
            public bool reach;
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

        public class WeaponTarget : Target
        {
            public int index;
            public AtkCondition attackableness;
        }
        public class Event : CommonEvent
        {
            public List<WeaponTarget> targets;
        }

        static List<Piece> defaultPattern = new List<Piece>
        {
            new Piece
            {
                pos = new Vector2(1, 0),
                dir = new Vector2(1, 0),
                reach = false
            }
        };
        List<Piece> pattern;
        Chain<CommonEvent> chain;
        Layer attackedLayer = Layer.REAL | Layer.MISC | Layer.WALL;

        public List<WeaponTarget> GetTargets(Entity actor, Action action)
        {
            var targets = new List<WeaponTarget>();
            double angle = Vector2.Right.AngleTo(actor.m_orientation);

            for (int i = 0; i < this.pattern.Count; i++)
            {
                var piece = this.pattern[i].Rotate(angle);
                var pos = actor.m_pos + piece.pos;
                var entity = actor.m_world.m_grid
                    .GetCellAt(pos)
                    .GetEntityFromLayer(attackedLayer);

                // TODO: refactor
                AtkCondition attackableness;
                if (entity != null)
                {
                    var attackable = entity.beh_Attackable;

                    attackableness =
                        attackable == null
                            ? AtkCondition.NEVER
                            : attackable.GetAttackableness();
                }
                else
                {
                    attackableness = AtkCondition.NEVER;
                }

                targets[i] = new WeaponTarget
                {
                    direction = piece.dir,
                    entity = entity,
                    index = i,
                    attackableness = attackableness
                };
            }

            var ev = new Event
            {
                targets = targets,
                actor = actor,
                action = action
            };

            chain.Pass(ev);

            return ev.targets;

        }
    }

    //     -- after that, analyze it
    //     local event = Event(actor, action)
    //     event.targets = map

    //     if self.check(event) then
    //         return event.targets
    //     end

    //     self.chain:pass(event, self.check)

    //     return event.targets
    // end
}