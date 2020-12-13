using System.Collections.Generic;
using System.Linq;
using Hopper.Utils;
using Hopper.Utils.Chains;

namespace Hopper.Core.Targeting
{
    public static class Handlers
    {
        public static void LeaveAttackableAndBlocks(TargetEvent<AtkTarget> weaponEvent)
        {
            weaponEvent.targets = weaponEvent.targets
                .Where(t => t.attackness.Is(Attackness.CAN_BE_ATTACKED | Attackness.IS_BLOCK));
            System.Console.WriteLine($"Target length: {weaponEvent.targets.Count}");
        }

        public static void TakeFirst_ThatCanBeAttacked_ByDefault(TargetEvent<AtkTarget> weaponEvent)
        {
            // take first that can be attacked by default
            AtkTarget first = weaponEvent.targets.Find(
                // we consider maybe to be
                t => t.attackness.Is(Attackness.CAN_BE_ATTACKED | Attackness.BY_DEFAULT)
            );

            weaponEvent.targets.Clear();

            if (first != null
                && (!first.attackness.Is(Attackness.IF_NEXT_TO) || first.piece.index == 0))
            {
                weaponEvent.targets.Add(first);
            }
        }

        public static void TakeAll_ThatCanBeAttacked_ByDefault(TargetEvent<AtkTarget> weaponEvent)
        {
            // take first that can be attacked by default
            var targets = weaponEvent.targets.Where(
                // we consider maybe to be
                t => t.attackness.Is(Attackness.CAN_BE_ATTACKED | Attackness.BY_DEFAULT)
            );

            if (
                // if all are attackable only close
                targets.All(t => t.attackness.Is(Attackness.IF_NEXT_TO))
                // and none are close
                && targets.None(t => t.piece.index == 0)
            )
            {
                weaponEvent.targets.Clear();
            }
            else
            {
                weaponEvent.targets = targets;
            }
        }

        public static void DiscardUnreachable<T>(TargetEvent<T> weaponEvent)
            where T : AtkTarget
        {
            // we assume they are sorted
            // weaponEvent.targets.Sort((a, b) => a.piece.index - b.piece.index);

            bool[] reachArray = new bool[weaponEvent.targets.Max(t => t.piece.index) + 1];
            reachArray.Fill(true);
            var newTargets = new List<T>();

            foreach (var target in weaponEvent.targets)
            {
                int i = target.piece.index;
                var piece = target.piece;

                // we prevent reach if we are a block
                reachArray[i] = target.attackness.Is(Attackness.IS_BLOCK);

                if (
                    // always reaches
                    piece.reach == null

                    // reaches only if all previous ones were not blocked
                    || piece.reach.Length == 0 && reachArray.Take(i).All(true)

                    // reaches in the specified ones were not blocked
                    || piece.reach.All(j => reachArray[j] == true))
                {
                    newTargets.Add(weaponEvent.targets[i]);
                }
            }

            weaponEvent.targets = newTargets;
        }

        public static Chain<TargetEvent<AtkTarget>> DefaultAtkChain;
        public static Chain<TargetEvent<AtkTarget>> MultiAtkChain;

        static Handlers()
        {
            DefaultAtkChain = new Chain<TargetEvent<AtkTarget>>();
            DefaultAtkChain.AddHandler(LeaveAttackableAndBlocks);
            DefaultAtkChain.AddHandler(DiscardUnreachable);
            DefaultAtkChain.AddHandler(TakeFirst_ThatCanBeAttacked_ByDefault);

            MultiAtkChain = new Chain<TargetEvent<AtkTarget>>();
            MultiAtkChain.AddHandler(LeaveAttackableAndBlocks);
            MultiAtkChain.AddHandler(DiscardUnreachable);
            MultiAtkChain.AddHandler(TakeAll_ThatCanBeAttacked_ByDefault);
        }
    }
}