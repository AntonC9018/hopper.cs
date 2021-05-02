using System.Collections.Generic;
using System.Linq;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Predictions;
using Hopper.Utils;
using Hopper.Utils.Vector;

namespace Hopper.Core
{
    public delegate IEnumerable<IntVector2> DirectedPredict(Entity actor, IntVector2 direction);
    public delegate IEnumerable<IntVector2> UndirectedPredict(Entity actor);
    public delegate bool DirectedDo(Entity actor, IntVector2 direction);
    public delegate bool UndirectedDo(Entity actor);

    public abstract class Action
    {
        public abstract ParticularAction ToParticular();

        public static DirectedAction CreateCompositeDirected(params DirectedAction[] actions)
        {
            var functions = (from action in actions select action.function).ToArray();
            var predicts = (from action in actions where action.predict != null select action.predict).ToArray();

            return CreateCompositeDirected(functions, predicts.Length > 0 ? predicts : null);
        }

        public static DirectedAction CreateCompositeDirected(DirectedDo[] functions, DirectedPredict[] predicts = null)
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

        public static UndirectedAction CreateCompositeUndirected(UndirectedDo[] functions, UndirectedPredict[] predicts = null)
        {
            var action = new UndirectedAction();

            action.function = (acting) =>
            {
                foreach (var function in functions)
                {
                    if (function(acting))
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

        public static DirectedAction CreateJoinedDirected(DirectedDo[] functions, DirectedPredict[] predicts = null)
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

        public static DirectedAction CreateSimple(DirectedDo function, DirectedPredict predict = null)
        {
            return new DirectedAction
            {
                function = function,
                predict = predict
            };
        }

        public static DirectedAction CreateSimple(System.Action<IntVector2, IntVector2> function, DirectedPredict predict = null)
        {
            return new DirectedAction
            {
                function = (actor, direction) => 
                { 
                    if (actor.TryGetTransform(out var transform)) 
                    {
                        function(transform.position, direction); 
                        return true; 
                    }
                    return false;
                },
                predict = predict
            };
        }

        public static DirectedAction CreateSimple(System.Action<Entity, IntVector2> function, DirectedPredict predict = null)
        {
            return new DirectedAction
            {
                function = (actor, d) => { function(actor, d); return true; },
                predict = predict
            };
        }

        public static UndirectedAction CreateSimple(UndirectedDo function, UndirectedPredict predict = null)
        {
            return new UndirectedAction
            {
                function = function,
                predict = predict
            };
        }

        public static UndirectedAction CreateSimple(System.Action<Entity> function, UndirectedPredict predict = null)
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

        public static UndirectedAction CreateJoinedUndirected(UndirectedDo[] functions, UndirectedPredict[] predicts = null)
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

        public static DirectedAction FromActivateable<T>(Index<T> index)
            where T : IComponent, IStandartActivateable
        {
            var action = new DirectedAction();
            action.function = (entity, d) => Activate(index, entity, d);

            if (typeof(IBehaviorPredictable).IsAssignableFrom(typeof(T)))
            {
                action.predict = (entity, d) => PredictVia(index, entity, d);
            }

            return action;
        }

        private static void FillInPredicts(DirectedAction action, DirectedPredict[] predicts)
        {
            if (predicts != null)
            {
                action.predict = new ClosureDirectedPredicts { predicts = predicts }.PredictAll;
            }
        }

        private static void FillInPredicts(UndirectedAction action, UndirectedPredict[] predicts)
        {
            if (predicts != null)
            {
                action.predict = new ClosureUndirectedPredicts { predicts = predicts }.PredictAll;
            }
        }

        private class ClosureDirectedPredicts
        {
            public DirectedPredict[] predicts;

            public IEnumerable<IntVector2> PredictAll(Entity actor, IntVector2 direction)
            {
                foreach (var predict in predicts)
                {
                    foreach (var vec in predict(actor, direction))
                    {
                        yield return vec;
                    }
                }
            }
        }

        private class ClosureUndirectedPredicts
        {
            public UndirectedPredict[] predicts;

            public IEnumerable<IntVector2> PredictAll(Entity actor)
            {
                foreach (var predict in predicts)
                {
                    foreach (var vec in predict(actor))
                    {
                        yield return vec;
                    }
                }
            }
        }

        private static bool Activate<T>(Index<T> index, Entity actor, IntVector2 direction)
            where T : IComponent, IStandartActivateable
        {
            Assert.That(actor.HasComponent(index), "Cannot execute action if the target behavior is missing");
            return actor.GetComponent(index).Activate(actor, direction);
        }

        private static IEnumerable<IntVector2> PredictVia<T>(Index<T> index, Entity actor, IntVector2 direction)
            where T : IComponent, IStandartActivateable
        {
            Assert.That(actor.HasComponent(index), "Cannot predict if the target behavior is missing");
            return ((IBehaviorPredictable)actor.GetComponent(index)).Predict(actor, direction);
        }
    }
}