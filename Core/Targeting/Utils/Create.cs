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

        public static TargetProvider<T, M> CreateMulti<T, M>(
            Pattern pattern,
            Chain<TargetEvent<T>> chain,
            System.Func<TargetEvent<T>, bool> stop,
            Layer skipLayer,
            Layer targetedLayer
        )
            where T : Target, ITarget<T, M>, new()
        {
            return new TargetProvider<T, M>(
                pattern, chain, stop,
                new MultiCalculator<T, MultiTarget<T, M>, M>(skipLayer, targetedLayer));
        }

        public static TargetProvider<T, M> CreateMulti<T, M>(
            Pattern pattern,
            Chain<TargetEvent<T>> chain,
            Layer skipLayer,
            Layer targetedLayer
        )
            where T : Target, ITarget<T, M>, new()
        {
            return CreateMulti<T, M>(
                pattern, chain, DefaultStop, skipLayer, targetedLayer);
        }

        public static TargetProvider<T, M> CreateMulti<T, U, M>(
            Pattern pattern,
            Chain<TargetEvent<T>> chain,
            System.Func<TargetEvent<T>, bool> stop,
            Layer skipLayer,
            Layer targetedLayer
        )
            where T : Target, ITarget<T, M>, new()
            where U : MultiTarget<T, M>, new()
        {
            return new TargetProvider<T, M>(
                pattern, chain, stop,
                new MultiCalculator<T, U, M>(skipLayer, targetedLayer));
        }

        public static TargetProvider<T, M> CreateMulti<T, U, M>(
            Pattern pattern,
            Chain<TargetEvent<T>> chain,
            Layer skipLayer,
            Layer targetedLayer
        )
            where T : Target, ITarget<T, M>, new()
            where U : MultiTarget<T, M>, new()
        {
            return CreateMulti<T, U, M>(pattern, chain, DefaultStop, skipLayer, targetedLayer);
        }

        public static TargetProvider<T, M> CreateSimple<T, M>(
            Pattern pattern,
            Chain<TargetEvent<T>> chain,
            Layer skipLayer,
            Layer targetedLayer)
            where T : Target, ITarget<T, M>, new()
        {
            return new TargetProvider<T, M>(pattern, chain, DefaultStop,
                new SimpleCalculator<T, M>(skipLayer, targetedLayer));
        }

        public static TargetProvider<T, M> CreateSimple<T, M>(
            Pattern pattern,
            Chain<TargetEvent<T>> chain,
            System.Func<TargetEvent<T>, bool> stop,
            Layer skipLayer,
            Layer targetedLayer
        )
            where T : Target, ITarget<T, M>, new()
        {
            return new TargetProvider<T, M>(pattern, chain, stop, new SimpleCalculator<T, M>(skipLayer, targetedLayer));
        }

        public static TargetProvider<AtkTarget, Attackable.Params> CreateAtk(
            Pattern pattern,
            Chain<TargetEvent<AtkTarget>> chain
        )
        {
            return new TargetProvider<AtkTarget, Attackable.Params>(
                pattern, chain, DefaultStop, new SimpleCalculator<AtkTarget, Attackable.Params>(Layer.WALL | Layer.DIRECTIONAL_WALL, Layer.REAL));
        }

        public static TargetProvider<AtkTarget, Attackable.Params> CreateAtk(
            Pattern pattern,
            Chain<TargetEvent<AtkTarget>> chain,
            System.Func<TargetEvent<AtkTarget>, bool> stop
        )
        {
            return new TargetProvider<AtkTarget, Attackable.Params>(
                pattern, chain, stop, new SimpleCalculator<AtkTarget, Attackable.Params>(Layer.WALL | Layer.DIRECTIONAL_WALL, Layer.REAL));
        }

        public static TargetProvider<DigTarget, Dig> CreateDig(
            Pattern pattern,
            Chain<TargetEvent<DigTarget>> chain
        )
        {
            return new TargetProvider<DigTarget, Dig>(
                pattern, chain, DefaultStop, new SimpleCalculator<DigTarget, Dig>(0, Layer.WALL));
        }

        public static TargetProvider<DigTarget, Dig> CreateDig(
            Pattern pattern,
            Chain<TargetEvent<DigTarget>> chain,
            System.Func<TargetEvent<DigTarget>, bool> stop
        )
        {
            return new TargetProvider<DigTarget, Dig>(
                pattern, chain, stop, new SimpleCalculator<DigTarget, Dig>(0, Layer.WALL));
        }
    }
}