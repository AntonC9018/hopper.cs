using Chains;
using Core.Behaviors;
using Core.Stats.Basic;

namespace Core.Targeting
{
    public static class TargetProvider
    {
        public static bool DefaultStop<T>(TargetEvent<T> e)
            where T : Target, new()
        {
            return (e.propagate == false) || e.targets.Count == 0;
        }

        public static BufferedAtkTargetProvider CreateAtk(
            IPattern pattern,
            Chain<TargetEvent<AtkTarget>> chain
        )
        {
            return new BufferedAtkTargetProvider(
                pattern, chain, DefaultStop, Layer.WALL, Layer.REAL);
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

        public static SimpleDigTargetProvider CreateSimpleDig()
        {
            return new SimpleDigTargetProvider();
        }
    }
}