using System.Collections.Generic;
using Vector;
using Chains;

namespace Core.Weapon
{
    class Weapon
    {
        public static double AngleBetween(Vector2 vector1, Vector2 vector2)
        {
            double sin = vector1.x * vector2.y - vector2.x * vector1.y;
            double cos = vector1.x * vector2.x + vector1.y * vector2.y;

            return System.Math.Atan2(sin, cos);
        }

        public class Piece
        {
            public Vector2 pos;
            public Vector2 dir;
            public bool reach;
        }

        public class WeaponTarget : Target
        {
            public int index;
            public Attackable.Attackableness attackableness;
        }

        static Vector2 right = new Vector2(1, 0);
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

        public class TargetsEvent : CommonEvent
        {
            public List<Target> targets;
        }

        public List<Target> GetTargets(Entity actor, Action action)
        {
            var targets = new List<Target>();
            double angle = AngleBetween(right, actor.m_orientation);

            for (int i = 0; i < this.pattern.Count; i++)
            {
                var piece = this.pattern[i];
                var pos = actor.m_pos + piece.pos;
                var entity = actor.m_world.m_grid
                    .GetCellAt(pos)
                    .GetEntityFromLayer(attackedLayer);

                // TODO: refactor
                Attackable.Attackableness attackableness;
                if (entity != null)
                {
                    var attackable = entity.beh_Attackable;

                    attackableness =
                        attackable == null
                            ? Attackable.Attackableness.UNATTACKABLE
                            : attackable.GetAttackableness();
                }
                else
                {
                    attackableness = Attackable.Attackableness.UNATTACKABLE;
                }

                targets[i] = new WeaponTarget
                {
                    direction = piece.dir,
                    entity = entity,
                    index = i,
                    attackableness = attackableness
                };
            }

            var ev = new TargetsEvent
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