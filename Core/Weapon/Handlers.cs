using System.Collections.Generic;
using System.Linq;
using Chains;
using Core;
using MyLinkedList;

namespace Core.Weapon
{
    public static class Handlers
    {
        public static void NextToAny(Weapon<Weapon.AtkTarget>.Event weaponEvent)
        {
            var first = weaponEvent.targets[0];
            if (first.index == 0 && first.atkCondition != AtkCondition.NEVER)
                return;

            if (weaponEvent.targets.Any(
                    t => t.atkCondition == AtkCondition.IF_NEXT_TO))
            {
                weaponEvent.propagate = false;
                weaponEvent.targets.Clear();
            }
        }

        public static void TakeFirstNotSkip(Weapon<Weapon.AtkTarget>.Event weaponEvent)
        {
            if (weaponEvent.targets.Count == 0)
                return;

            // take first that doesn't have low priority
            Weapon.AtkTarget first = weaponEvent.targets.Find(
                t => t.atkCondition != AtkCondition.SKIP);

            // if all have low priority, take the first one
            if (first == null)
                first = weaponEvent.targets[0];

            weaponEvent.targets.Clear();
            weaponEvent.targets.Add(first);
        }

        public static void DiscardUnreachable<T>(Weapon<T>.Event weaponEvent)
            where T : Weapon.Target, new()
        {
            weaponEvent.targets = weaponEvent.targets
                .Where(t => CanReach(t, weaponEvent.targets));
        }

        public static bool CanReach<T>(T target, List<T> targets) where T : Weapon.Target
        {
            var reach = target.initialPiece.reach;

            // always reachable
            if (reach == null)
                return true;

            // reachable only if all the ones before are empty
            if (reach.Count == 0)
                // is of lowest index
                return !targets.Any(t => t.index < target.index);

            // reachable if no specified indeces are present
            return !targets.Any(t => reach.Contains(t.index));
        }

        public static void DiscardNotClose(Weapon<Weapon.AtkTarget>.Event weaponEvent)
        {
            weaponEvent.targets = weaponEvent.targets
                .FilterFromIndex(t => t.atkCondition != AtkCondition.IF_NEXT_TO, 1);
        }

        public static void DiscardUnattackable(Weapon<Weapon.AtkTarget>.Event weaponEvent)
        {
            weaponEvent.targets = weaponEvent.targets
                .Where(t => t.atkCondition != AtkCondition.NEVER);
        }

        public static void DiscardNoEntity<T>(Weapon<T>.Event weaponEvent)
            where T : Weapon.Target, new()
        {
            weaponEvent.targets = weaponEvent.targets
                .Where(t => t.entity != null);
        }

        public static void KeepAttackable(Weapon<Weapon.AtkTarget>.Event weaponEvent)
        {
            weaponEvent.targets = weaponEvent.targets
                .Where(t => t.atkCondition == AtkCondition.ALWAYS
                         || t.atkCondition == AtkCondition.IF_NEXT_TO);
        }

        public static Chain<Weapon<Weapon.AtkTarget>.Event> GeneralChain;

        static Handlers()
        {
            var list = new List<System.Action<Weapon<Weapon.AtkTarget>.Event>>{
                DiscardNoEntity,
                NextToAny,
                DiscardUnreachable,
                DiscardUnattackable,
                DiscardNotClose,
                TakeFirstNotSkip
            };
            GeneralChain = new Chain<Weapon<Weapon.AtkTarget>.Event>();
            foreach (var func in list)
            {
                GeneralChain.AddHandler(new EvHandler<Weapon<Weapon.AtkTarget>.Event>(func));
            }

            // GeneralChain.AddHandler(new EvHandler<Weapon<Weapon.AtkTarget>.Event>(DiscardNoEntity));
            // GeneralChain.AddHandler(new EvHandler<Weapon<Weapon.AtkTarget>.Event>(NextToAny));
            // GeneralChain.AddHandler(new EvHandler<Weapon<Weapon.AtkTarget>.Event>(DiscardUnreachable));
            // GeneralChain.AddHandler(new EvHandler<Weapon<Weapon.AtkTarget>.Event>(DiscardUnattackable));
            // GeneralChain.AddHandler(new EvHandler<Weapon<Weapon.AtkTarget>.Event>(DiscardNotClose));
            // GeneralChain.AddHandler(new EvHandler<Weapon<Weapon.AtkTarget>.Event>(TakeFirstNotSkip));
        }
    }
}