using Hopper.Utils.Chains;
using Hopper.Core.Behaviors;
using Hopper.Core.Stats.Basic;

namespace Hopper.Core.Targeting
{
    public static class TargetProvider
    {
        public static bool DefaultStop<T>(TargetEvent<T> e)
        {
            return (e.propagate == false) || e.targets.Count == 0;
        }

        public static BufferedAtkTargetProvider CreateAtk(
            IPattern pattern,
            StaticChain<TargetEvent<AtkTarget>> chain
        )
        {
            return new BufferedAtkTargetProvider(
                pattern, chain, DefaultStop<AtkTarget>, Layer.REAL | Layer.WALL);
        }

        public static BufferedAtkTargetProvider CreateAtk(
            IPattern pattern,
            StaticChain<TargetEvent<AtkTarget>> chain,
            System.Func<TargetEvent<AtkTarget>, bool> stop
        )
        {
            return new BufferedAtkTargetProvider(
                pattern, chain, stop, Layer.REAL | Layer.WALL);
        }

        public static readonly SingleTargetProvider SimpleDig = new SingleTargetProvider(0, Layer.WALL);
        public static readonly SingleAtkTargetProvider SimpleAttack = new SingleAtkTargetProvider(Layer.WALL, Layer.REAL);
    }
}