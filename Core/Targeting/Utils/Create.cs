using Chains;
using Core.Behaviors;
using Core.Stats.Basic;

namespace Core.Targeting
{
    public static class TargetProvider
    {
        public static bool DefaultStop<T>(TargetEvent<T> e)
        {
            return (e.propagate == false) || e.targets.Count == 0;
        }

        public static BufferedAtkTargetProvider CreateAtk(
            IPattern pattern,
            Chain<TargetEvent<AtkTarget>> chain
        )
        {
            return new BufferedAtkTargetProvider(
                pattern, chain, DefaultStop<AtkTarget>, Layer.WALL, Layer.REAL);
        }

        public static BufferedAtkTargetProvider CreateAtk(
            IPattern pattern,
            Chain<TargetEvent<AtkTarget>> chain,
            System.Func<TargetEvent<AtkTarget>, bool> stop
        )
        {
            return new BufferedAtkTargetProvider(
                pattern, chain, stop, Layer.WALL, Layer.REAL);
        }

        public static readonly SimpleTargetProvider SimpleDig = new SimpleTargetProvider(0, Layer.WALL);
        public static readonly SimpleAtkTargetProvider SimpleAttack = new SimpleAtkTargetProvider(Layer.WALL, Layer.REAL);
    }
}