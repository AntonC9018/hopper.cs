using System.Collections.Generic;
using System.Linq;
using Hopper.Core.Utils;
using Chains;

namespace Hopper.Core.Targeting
{
    public static class Handlers
    {
        public static void NextToAny(TargetEvent<AtkTarget> weaponEvent)
        {
            var first = weaponEvent.targets[0];
            if (first.piece.index == 0 && first.atkCondition != Attackness.NEVER)
                return;

            if (weaponEvent.targets.Any(
                    t => t.atkCondition == Attackness.IF_NEXT_TO))
            {
                weaponEvent.propagate = false;
                weaponEvent.targets.Clear();
            }
        }

        public static void TakeFirstNotSkip(TargetEvent<AtkTarget> weaponEvent)
        {
            if (weaponEvent.targets.Count == 0)
                return;

            // take first that doesn't have low priority
            AtkTarget first = weaponEvent.targets.Find(
                t => t.atkCondition != Attackness.SKIP);

            // if all have low priority, take the first one
            if (first == null)
                first = weaponEvent.targets[0];

            weaponEvent.targets.Clear();
            weaponEvent.targets.Add(first);
        }

        public static void DiscardUnreachable<T>(TargetEvent<T> weaponEvent)
            where T : AtkTarget
        {
            weaponEvent.targets = weaponEvent.targets
                .Where(t => CanReach(t, weaponEvent.targets));
        }

        public static bool CanReach<T>(T target, IEnumerable<T> targets)
            where T : AtkTarget
        {
            var reach = target.piece.reach;

            // always reachable
            if (reach == null)
                return true;

            // reachable only if all the ones before are empty
            if (reach.Count == 0)
                // is of lowest index
                return !targets.Any(t => t.piece.index < target.piece.index);

            // reachable if no specified indeces are present
            return !targets.Any(t => reach.Contains(t.piece.index));
        }

        public static void DiscardNotClose(TargetEvent<AtkTarget> weaponEvent)
        {
            weaponEvent.targets = weaponEvent.targets
                .FilterFromIndex(t => t.atkCondition != Attackness.IF_NEXT_TO, 1);
        }

        public static void DiscardUnattackable(TargetEvent<AtkTarget> weaponEvent)
        {
            weaponEvent.targets = weaponEvent.targets
                .Where(t => t.atkCondition != Attackness.NEVER);
        }

        public static void DiscardNoEntity<T>(TargetEvent<T> weaponEvent)
            where T : AtkTarget
        {
            weaponEvent.targets = weaponEvent.targets
                .Where(t => t.targetEntity != null);
        }

        public static void KeepAttackable(TargetEvent<AtkTarget> weaponEvent)
        {
            weaponEvent.targets = weaponEvent.targets
                .Where(t => t.atkCondition == Attackness.ALWAYS
                         || t.atkCondition == Attackness.IF_NEXT_TO);
        }

        public static Chain<TargetEvent<AtkTarget>> GeneralChain;

        static Handlers()
        {
            var generalHandlers = new List<System.Action<TargetEvent<AtkTarget>>>{
                DiscardNoEntity,
                NextToAny,
                DiscardUnreachable,
                DiscardUnattackable,
                DiscardNotClose,
                TakeFirstNotSkip,
            };
            GeneralChain = new Chain<TargetEvent<AtkTarget>>();
            foreach (var func in generalHandlers)
            {
                GeneralChain.AddHandler(func);
            }
        }
    }
}