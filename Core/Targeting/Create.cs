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

        public static TargetProvider<T, E> CreateMulti<T, U, E>(
            Pattern pattern,
            Chain<E> chain,
            System.Func<E, bool> stop
        )
            where T : Target, new()
            where U : MultiTarget<T, E>, new()
            where E : TargetEvent<T>
        {
            return new TargetProvider<T, E>(pattern, chain, stop, new MultiCalculator<T, U, E>());
        }

        public static TargetProvider<T, E> CreateMulti<T, U, E>(
            Pattern pattern,
            Chain<E> chain
        )
            where T : Target, new()
            where U : MultiTarget<T, E>, new()
            where E : TargetEvent<T>
        {
            return new TargetProvider<T, E>(pattern, chain, DefaultStop, new MultiCalculator<T, U, E>());
        }

        public static TargetProvider<T, E> CreateSimple<T, E>(
            Pattern pattern,
            Chain<E> chain)
            where T : Target, ITarget<T, E>, new()
            where E : TargetEvent<T>
        {
            return new TargetProvider<T, E>(pattern, chain, DefaultStop, new SimpleCalculator<T, E>());
        }

        public static TargetProvider<T, E> CreateSimple<T, E>(
            Pattern pattern,
            Chain<E> chain,
            System.Func<E, bool> stop
        )
            where T : Target, ITarget<T, E>, new()
            where E : TargetEvent<T>
        {
            return new TargetProvider<T, E>(pattern, chain, stop, new SimpleCalculator<T, E>());
        }

        public static TargetProvider<AtkTarget, AtkTargetEvent> CreateAtk(
            Pattern pattern,
            Chain<AtkTargetEvent> chain
        )
        {
            return new TargetProvider<AtkTarget, AtkTargetEvent>(pattern, chain, DefaultStop, new SimpleCalculator<AtkTarget, AtkTargetEvent>());
        }

        public static TargetProvider<AtkTarget, AtkTargetEvent> CreateAtk(
            Pattern pattern,
            Chain<AtkTargetEvent> chain,
            System.Func<AtkTargetEvent, bool> stop
        )
        {
            return new TargetProvider<AtkTarget, AtkTargetEvent>(pattern, chain, stop, new SimpleCalculator<AtkTarget, AtkTargetEvent>());
        }

        public static TargetProvider<DigTarget, DigTargetEvent> CreateDig(
            Pattern pattern,
            Chain<DigTargetEvent> chain
        )
        {
            return new TargetProvider<DigTarget, DigTargetEvent>(pattern, chain, DefaultStop, new SimpleCalculator<DigTarget, DigTargetEvent>());
        }

        public static TargetProvider<DigTarget, DigTargetEvent> CreateDig(
            Pattern pattern,
            Chain<DigTargetEvent> chain,
            System.Func<DigTargetEvent, bool> stop
        )
        {
            return new TargetProvider<DigTarget, DigTargetEvent>(pattern, chain, stop, new SimpleCalculator<DigTarget, DigTargetEvent>());
        }
    }
}