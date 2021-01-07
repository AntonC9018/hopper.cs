using System.Collections.Generic;
using System.Linq;
using Hopper.Core.Behaviors;
using Hopper.Core.Predictions;
using Hopper.Utils;
using Hopper.Utils.Vector;

namespace Hopper.Core
{
    public delegate IEnumerable<IntVector2> PredictDirected(Entity entity, IntVector2 direction);
    public delegate IEnumerable<IntVector2> PredictUndirected(Entity entity);
    public delegate bool DirectedDo(Entity entity, IntVector2 direction);
    public delegate bool UndirectedDo(Entity entity);

    public abstract class Action
    {
        public abstract ParticularAction ToParticular();

        public static DirectedAction CreateCompositeDirected(params DirectedAction[] actions)
        {
            var functions = (from action in actions select action.function).ToArray();
            var predicts = (from action in actions where action.predict != null select action.predict).ToArray();

            return CreateCompositeDirected(functions, predicts.Length > 0 ? predicts : null);
        }

        public static DirectedAction CreateCompositeDirected(DirectedDo[] functions, PredictDirected[] predicts = null)
        {
            var action = new DirectedAction();

            action.function = (entity, direction) =>
            {
                foreach (var function in functions)
                {
                    if (function(entity, direction))
                    {
                        return true;
                    }
                }
                return false;
            };
            FillInPredicts(action, predicts);

            return action;
        }

        public static UndirectedAction CreateCompositeUndirected(params UndirectedAction[] actions)
        {
            var functions = (from action in actions select action.function).ToArray();
            var predicts = (from action in actions where action.predict != null select action.predict).ToArray();

            return CreateCompositeUndirected(functions, predicts.Length > 0 ? predicts : null);
        }

        public static UndirectedAction CreateCompositeUndirected(UndirectedDo[] functions, PredictUndirected[] predicts = null)
        {
            var action = new UndirectedAction();

            action.function = (entity) =>
            {
                foreach (var function in functions)
                {
                    if (function(entity))
                    {
                        return true;
                    }
                }
                return false;
            };
            FillInPredicts(action, predicts);

            return action;
        }

        public static DirectedAction CreateJoinedDirected(params DirectedAction[] actions)
        {
            var functions = (from action in actions select action.function).ToArray();
            var predicts = (from action in actions where action.predict != null select action.predict).ToArray();

            return CreateJoinedDirected(functions, predicts.Length > 0 ? predicts : null);
        }

        public static DirectedAction CreateJoinedDirected(DirectedDo[] functions, PredictDirected[] predicts = null)
        {
            var action = new DirectedAction();

            action.function = (entity, direction) =>
            {
                bool success = false;
                foreach (var function in functions)
                {
                    if (function(entity, direction))
                    {
                        success = true;
                    }
                }
                return success;
            };
            FillInPredicts(action, predicts);

            return action;
        }

        public static DirectedAction CreateSimple(DirectedDo function, PredictDirected predict = null)
        {
            return new DirectedAction
            {
                function = function,
                predict = predict
            };
        }

        public static DirectedAction CreateSimple(System.Action<Entity, IntVector2> function, PredictDirected predict = null)
        {
            return new DirectedAction
            {
                function = (e, d) => { function(e, d); return true; },
                predict = predict
            };
        }

        public static UndirectedAction CreateSimple(UndirectedDo function, PredictUndirected predict = null)
        {
            return new UndirectedAction
            {
                function = function,
                predict = predict
            };
        }

        public static UndirectedAction CreateSimple(System.Action<Entity> function, PredictUndirected predict = null)
        {
            return new UndirectedAction
            {
                function = (e) => { function(e); return true; },
                predict = predict
            };
        }

        public static UndirectedAction CreateJoinedUndirected(params UndirectedAction[] actions)
        {
            var functions = (from action in actions select action.function).ToArray();
            var predicts = (from action in actions where action.predict != null select action.predict).ToArray();

            return CreateJoinedUndirected(functions, predicts.Length > 0 ? predicts : null);
        }

        public static UndirectedAction CreateJoinedUndirected(UndirectedDo[] functions, PredictUndirected[] predicts = null)
        {
            var action = new UndirectedAction();

            action.function = (entity) =>
            {
                bool success = false;
                foreach (var function in functions)
                {
                    if (function(entity))
                    {
                        success = true;
                    }
                }
                return success;
            };
            FillInPredicts(action, predicts);

            return action;
        }

        public static DirectedAction CreateBehavioral<T>()
            where T : Behavior, IStandartActivateable
        {
            var action = new DirectedAction();
            action.function = ActivateBehavior<T>;

            if (typeof(IDirectedPredictable).IsAssignableFrom(typeof(T)))
            {
                action.predict = PredictViaBehavior<T>;
            }

            return action;
        }

        private static void FillInPredicts(DirectedAction action, PredictDirected[] predicts)
        {
            if (predicts != null)
            {
                action.predict = new ClosureDirectedPredicts { predicts = predicts }.PredictAll;
            }
        }

        private static void FillInPredicts(UndirectedAction action, PredictUndirected[] predicts)
        {
            if (predicts != null)
            {
                action.predict = new ClosureUndirectedPredicts { predicts = predicts }.PredictAll;
            }
        }

        private class ClosureDirectedPredicts
        {
            public PredictDirected[] predicts;

            public IEnumerable<IntVector2> PredictAll(Entity entity, IntVector2 direction)
            {
                foreach (var predict in predicts)
                {
                    foreach (var vec in predict(entity, direction))
                    {
                        yield return vec;
                    }
                }
            }
        }

        private class ClosureUndirectedPredicts
        {
            public PredictUndirected[] predicts;

            public IEnumerable<IntVector2> PredictAll(Entity entity)
            {
                foreach (var predict in predicts)
                {
                    foreach (var vec in predict(entity))
                    {
                        yield return vec;
                    }
                }
            }
        }

        private static bool ActivateBehavior<T>(Entity entity, IntVector2 direction)
            where T : Behavior, IStandartActivateable
        {
            Assert.That(entity.Behaviors.Has<T>(), "Cannot execute action if the target behavior is missing");
            return entity.Behaviors.Get<T>().Activate(direction);
        }

        private static IEnumerable<IntVector2> PredictViaBehavior<T>(Entity entity, IntVector2 direction)
            where T : Behavior, IStandartActivateable
        {
            Assert.That(entity.Behaviors.Has<T>(), "Cannot predict if the target behavior is missing");
            Assert.That(entity.Behaviors.Get<T>() is IDirectedPredictable, "Cannot predict if the target behavior is not predictable");
            return ((IDirectedPredictable)entity.Behaviors.Get<T>()).GetPositions(direction);
        }
    }
}