using System;
using System.Collections.Generic;
using System.Linq;
using Hopper.Core.ActingNS;
using Hopper.Core.Components;
using Hopper.Core.WorldNS;
using Hopper.Utils;
using Hopper.Utils.Vector;

namespace Hopper.Core.ActingNS
{
    public static class IActionExtensions
    {
        public static CompiledAction Compile(this IAction action, IntVector2 direction) 
            => new CompiledAction(action, direction);

        public static CompiledAction Compile(this IUndirectedAction action) 
            => new CompiledAction(action, IntVector2.Zero);
        
        public static bool WhetherActivates(this IAction action, Identifier activateableId)
            => (action as IActivatingAction)?.GetIdOfActivateable() == activateableId;
        
        public static bool ActivatesEither(this IAction action, params Identifier[] activateableIds)
        {
            if (action is IActivatingAction activating)
            {
                return activateableIds.Contains(activating.GetIdOfActivateable());
            }
            return false;
        }
    }

    public interface IAction
    {
        bool DoAction(Entity actor, IntVector2 direction);
    }

    public interface IUndirectedAction : IAction
    {
        bool DoAction(Entity actor);
    }

    public interface IPredictable
    {
        IEnumerable<IntVector2> Predict(Entity actor, IntVector2 direction, PredictionTargetInfo info);
    }

    public interface IUndirectedPredictable : IPredictable
    {
        IEnumerable<IntVector2> Predict(Entity actor, PredictionTargetInfo info);
    }

    // TODO: should this be a readonly struct? probably
    public readonly struct CompiledAction
    {
        public readonly IAction _storedAction;
        public readonly IntVector2 direction;

        public CompiledAction(IAction storedAction, IntVector2 direction)
        {
            this._storedAction = storedAction;
            this.direction = direction;
        }

        public CompiledAction WithDirection(IntVector2 newDirection)
        {
            return new CompiledAction(_storedAction, newDirection);
        }

        public CompiledAction WithAction(IAction newAction)
        {
            return new CompiledAction(newAction, direction);
        }

        public bool DoAction(Entity actor)
        {
            if (_storedAction != null)
                return _storedAction.DoAction(actor, direction);
            return true;
        }

        public bool HasAction()
        {
            return !(_storedAction is null);
        }

        public IAction GetStoredAction()
        {
            return _storedAction;
        }

        public IUndirectedAction GetStoredActionAsUndirected()
        {
            return _storedAction as IUndirectedAction;
        }

        public static bool operator ==(CompiledAction action1, CompiledAction action2)
        {
            return action1.direction == action2.direction && action1._storedAction == action2._storedAction;
        }

        public static bool operator !=(CompiledAction action1, CompiledAction action2)
        {
            return !(action1 == action2);
        }

        public override int GetHashCode()
        {
            if (_storedAction is null)
            {
                return direction.GetHashCode();
            }
            return _storedAction.GetHashCode() ^ direction.GetHashCode(); 
        }

        public override bool Equals(object obj)
        {
            return (obj is CompiledAction action) && action == this;
        }
    }

    public class JoinedAction : IAction, IPredictable
    {
        public IAction[] actions;

        public JoinedAction(params IAction[] actions)
        {
            this.actions = actions;
        }

        public bool DoAction(Entity actor, IntVector2 direction)
        {
            bool success = false;
            foreach (var action in actions)
            {
                success = success || action.DoAction(actor, direction);
            }
            return success;
        }

        public IEnumerable<IntVector2> Predict(Entity actor, IntVector2 direction, PredictionTargetInfo info)
        {
            foreach (var action in actions)
            if (action is IPredictable predictable)
            foreach (var position in predictable.Predict(actor, direction, info))
            {
                yield return position;
            }
        }
    }

    public class CompositeAction : IAction, IPredictable
    {
        public IAction[] actions;

        public CompositeAction(params IAction[] actions)
        {
            this.actions = actions;
        }

        public bool DoAction(Entity actor, IntVector2 direction)
        {
            foreach (var action in actions)
            {
                if (action.DoAction(actor, direction)) return true;
            }
            return false;
        }

        public IEnumerable<IntVector2> Predict(Entity actor, IntVector2 direction, PredictionTargetInfo info)
        {
            foreach (var action in actions)
            if (action is IPredictable predictable)
            foreach (var position in predictable.Predict(actor, direction, info))
            {
                yield return position;
            }
        }
    }

    public class SimpleAction : IAction 
    {
        public DirectedDo DoFunc;

        public SimpleAction(DirectedDo DoFunc)
        {
            this.DoFunc = DoFunc;
        }

        public bool DoAction(Entity actor, IntVector2 direction)
        {
            return DoFunc(actor, direction);
        }
    }

    public class SimplePredictableAction : SimpleAction, IPredictable
    {
        public DirectedPredict PredictFunc;

        public SimplePredictableAction(DirectedPredict PredictFunc, DirectedDo DoFunc) : base(DoFunc)
        {
            this.PredictFunc = PredictFunc;
        }

        public IEnumerable<IntVector2> Predict(Entity actor, IntVector2 direction, PredictionTargetInfo info)
        {
            return PredictFunc(actor, direction, info);
        }
    }

    public class SimpleUndirectedAction : IUndirectedAction 
    {
        public UndirectedDo DoFunc;

        public SimpleUndirectedAction(UndirectedDo DoFunc)
        {
            this.DoFunc = DoFunc;
        }

        public bool DoAction(Entity actor)
        {
            return DoFunc(actor);
        }

        bool IAction.DoAction(Entity actor, IntVector2 direction) => DoAction(actor);
    }

    public class SimplePredictableUndirectedAction : SimpleUndirectedAction, IUndirectedPredictable
    {
        public UndirectedPredict PredictFunc;

        public SimplePredictableUndirectedAction(UndirectedPredict PredictFunc, UndirectedDo DoFunc) : base(DoFunc)
        {
            this.PredictFunc = PredictFunc;
        }

        IEnumerable<IntVector2> IPredictable.Predict(Entity actor, IntVector2 direction, PredictionTargetInfo info)
        {
            return Predict(actor, info);
        }

        public IEnumerable<IntVector2> Predict(Entity actor, PredictionTargetInfo info)
        {
            return PredictFunc(actor, info);
        }
    }
    
    public interface IActivatingAction
    {
        Identifier GetIdOfActivateable();
    }
    
    public interface IStandartActivateable
    {
        bool Activate(Entity entity, IntVector2 direction);
    }

    public interface IUndirectedActivateable
    {
        bool Activate(Entity entity);
    }

    public class ActivatingAction<T> : IAction, IActivatingAction where T : IStandartActivateable, IComponent 
    {
        public Index<T> activateableId;

        public ActivatingAction(Index<T> activateableId)
        {
            this.activateableId = activateableId;
        }

        public bool DoAction(Entity actor, IntVector2 direction)
        {
            return actor.GetComponent(activateableId).Activate(actor, direction);
        }

        public Identifier GetIdOfActivateable() => activateableId.Id;
    }

    public class UndirectedActivatingAction<T> : IUndirectedAction, IActivatingAction where T : IUndirectedActivateable, IComponent 
    {
        public Index<T> activateableId;

        public UndirectedActivatingAction(Index<T> activateableId)
        {
            this.activateableId = activateableId;
        }

        public bool DoAction(Entity actor)
        {
            return actor.GetComponent(activateableId).Activate(actor);
        }

        public Identifier GetIdOfActivateable() => activateableId.Id;
        bool IAction.DoAction(Entity actor, IntVector2 direction) => DoAction(actor);
    }

    public class PredictableActivatingAction<T> : ActivatingAction<T>, IPredictable where T : IStandartActivateable, IComponent, IPredictable
    {
        public PredictableActivatingAction(Index<T> activateableId) : base(activateableId)
        {
        }

        public IEnumerable<IntVector2> Predict(Entity actor, IntVector2 direction, PredictionTargetInfo info)
        {
            return actor.GetComponent(activateableId).Predict(actor, direction, info);
        }
    }

    public class UndirectedPredictableActivatingAction<T> : UndirectedActivatingAction<T>, IPredictable where T : IUndirectedActivateable, IComponent, IUndirectedPredictable
    {
        public UndirectedPredictableActivatingAction(Index<T> activateableId) : base(activateableId)
        {
        }

        public IEnumerable<IntVector2> Predict(Entity actor, IntVector2 direction, PredictionTargetInfo info) => Predict(actor, info);

        public IEnumerable<IntVector2> Predict(Entity actor, PredictionTargetInfo info)
        {
            return actor.GetComponent(activateableId).Predict(actor, info);
        }
    }

    public class ConditionalAction : IAction
    {
        public IAction IfAction;
        public IAction ThenAction;

        public ConditionalAction(IAction If, IAction Then)
        {
            Assert.That(If != null, "The conditional must not be null");
            IfAction = If;
            ThenAction = Then;
        }

        public bool DoAction(Entity actor, IntVector2 direction)
        {
            if (IfAction.DoAction(actor, direction))
            {
                if (ThenAction == null) return false;
                return ThenAction.DoAction(actor, direction);
            }
            return false;
        }
    }

    public delegate IEnumerable<IntVector2> DirectedPredict(Entity actor, IntVector2 direction,  PredictionTargetInfo info);
    public delegate IEnumerable<IntVector2> UndirectedPredict(Entity actor, PredictionTargetInfo info);
    public delegate bool DirectedDo(Entity actor, IntVector2 direction);
    public delegate bool UndirectedDo(Entity actor);

    // using static Hopper.Core.Action;
    public static class Action
    {
        public static PredictableActivatingAction<T> FromPredictableActivateable<T>(Index<T> index) 
            where T : IStandartActivateable, IComponent, IPredictable 
            => new PredictableActivatingAction<T>(index);

        public static ActivatingAction<T> FromActivateable<T>(Index<T> index) 
            where T : IStandartActivateable, IComponent 
            => new ActivatingAction<T>(index);

        public static UndirectedActivatingAction<T> FromUndirectedActivateable<T>(Index<T> index) 
            where T : IUndirectedActivateable, IComponent 
            => new UndirectedActivatingAction<T>(index);

        /// <summary>
        /// UPA for short, read either "yu-pee-ay" or "oo-pah"
        /// </summary>
        public static UndirectedPredictableActivatingAction<T> FromUndirectedPredictableActivateable<T>(Index<T> index) 
            where T : IUndirectedActivateable, IComponent, IUndirectedPredictable 
            => new UndirectedPredictableActivatingAction<T>(index);

        public static JoinedAction Join(params IAction[] actions) => new JoinedAction(actions);
        public static CompositeAction Compose(params IAction[] actions) => new CompositeAction(actions);

        public static DirectedDo Adapt(Action<Entity, IntVector2> Func) => 
            (e, d) => { Func(e, d); return true; };
        public static UndirectedDo Adapt(Action<Entity> Func) => 
            (e) => { Func(e); return true; };
        public static DirectedPredict Adapt(Func<Entity, IntVector2, IEnumerable<IntVector2>> Func) =>
            (e, d, info) => Func(e, d); 
        public static DirectedPredict Adapt(Func<IntVector2, IntVector2, IEnumerable<IntVector2>> Func) =>
            (e, d, info) => Func(e.GetTransform().position, d);
        public static UndirectedPredict Adapt(Func<Entity, IEnumerable<IntVector2>> Func) => 
            (e, info) => Func(e);
        public static UndirectedPredict Adapt(Func<IntVector2, IEnumerable<IntVector2>> Func) =>
            (e, info) => Func(e.GetTransform().position);

        public static SimpleAction Simple(DirectedDo DoFunc) => new SimpleAction(DoFunc);
        public static SimplePredictableAction Simple(DirectedDo DoFunc, DirectedPredict PredictFunc) 
            => new SimplePredictableAction(PredictFunc, DoFunc);
        public static SimpleUndirectedAction Simple(UndirectedDo DoFunc) 
            => new SimpleUndirectedAction(DoFunc);
        public static SimplePredictableUndirectedAction Simple(UndirectedDo DoFunc, UndirectedPredict PredictFunc) 
            => new SimplePredictableUndirectedAction(PredictFunc, DoFunc);

        public static ConditionalAction Then(this IAction If, IAction Then) => new ConditionalAction(If, Then);
    }
}