using Chains;

namespace Core.Targeting
{
    public static class TargetProvider
    {
        public static bool DefaultStop<T>(TargetEvent<T> e)
            where T : Target, new()
        {
            return (e.propagate == false) || e.targets.Count == 0;
        }

        public static TargetProvider<T> CreateMulti<T>(
            Pattern pattern,
            Chain<TargetEvent<T>> chain,
            System.Func<TargetEvent<T>, bool> stop
        )
            where T : Target, new()
        {
            return new TargetProvider<T>(pattern, chain, stop, new MultiCalculator<T>());
        }

        public static TargetProvider<T> CreateMulti<T>(
            Pattern pattern,
            Chain<TargetEvent<T>> chain
        )
            where T : Target, new()
        {
            return new TargetProvider<T>(pattern, chain, DefaultStop, new MultiCalculator<T>());
        }

        public static TargetProvider<T> CreateSimple<T>(
            Pattern pattern,
            Chain<TargetEvent<T>> chain)
            where T : Target, new()
        {
            return new TargetProvider<T>(pattern, chain, DefaultStop, new SimpleCalculator<T>());
        }

        public static TargetProvider<T> CreateSimple<T>(
            Pattern pattern,
            Chain<TargetEvent<T>> chain,
            System.Func<TargetEvent<T>, bool> stop
        )
            where T : Target, new()
        {
            return new TargetProvider<T>(pattern, chain, DefaultStop, new SimpleCalculator<T>());
        }
    }
}