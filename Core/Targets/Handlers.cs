using System.Collections.Generic;
using System.Linq;
using Chains;
using Core;
using MyLinkedList;
using Core.Behaviors;

namespace Core.Weapon
{
    public static class Handlers
    {
        public static void NextToAny(TargetEvent<Weapon.AtkTarget> weaponEvent)
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

        public static void TakeFirstNotSkip(TargetEvent<Weapon.AtkTarget> weaponEvent)
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

        public static void DiscardUnreachable<T>(TargetEvent<T> weaponEvent)
            where T : Weapon.Target
        {
            weaponEvent.targets = weaponEvent.targets
                .Where(t => CanReach(t, weaponEvent.targets));
        }

        public static bool CanReach<T>(T target, List<T> targets)
            where T : Weapon.Target
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

        public static void DiscardNotClose(TargetEvent<Weapon.AtkTarget> weaponEvent)
        {
            weaponEvent.targets = weaponEvent.targets
                .FilterFromIndex(t => t.atkCondition != AtkCondition.IF_NEXT_TO, 1);
        }

        public static void DiscardUnattackable(TargetEvent<Weapon.AtkTarget> weaponEvent)
        {
            weaponEvent.targets = weaponEvent.targets
                .Where(t => t.atkCondition != AtkCondition.NEVER);
        }

        public static void DiscardNoEntity<T>(TargetEvent<T> weaponEvent)
            where T : Weapon.Target
        {
            weaponEvent.targets = weaponEvent.targets
                .Where(t => t.entity != null);
        }

        public static void KeepAttackable(TargetEvent<Weapon.AtkTarget> weaponEvent)
        {
            weaponEvent.targets = weaponEvent.targets
                .Where(t => t.atkCondition == AtkCondition.ALWAYS
                         || t.atkCondition == AtkCondition.IF_NEXT_TO);
        }

        public static Chain<TargetEvent<Weapon.AtkTarget>> GeneralChain;

        static Handlers()
        {
            var list = new List<System.Action<TargetEvent<Weapon.AtkTarget>>>{
                DiscardNoEntity,
                NextToAny,
                DiscardUnreachable,
                DiscardUnattackable,
                DiscardNotClose,
                TakeFirstNotSkip
            };
            GeneralChain = new Chain<TargetEvent<Weapon.AtkTarget>>();
            foreach (var func in list)
            {
                GeneralChain.AddHandler(new EvHandler<TargetEvent<Weapon.AtkTarget>>(func));
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