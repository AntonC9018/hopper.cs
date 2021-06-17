using Hopper.Utils.Vector;
using System.Collections.Generic;
using Hopper.Shared.Attributes;
using Hopper.Utils.Chains;
using Hopper.Core.Components;
using Hopper.Core.WorldNS;
using Hopper.Core.Components.Basic;
using Hopper.Utils;

namespace Hopper.Core.ActingNS
{

    [Flags] public enum ActingState 
    {
        DidAction                   = 1,
        DoingAction                 = 1 << 1,
        ActionSucceeded             = 1 << 2,
        ActionSet                   = 1 << 3,
        ActionSubstituted           = 1 << 4,
        ActionSubstitutionTraversed = 1 << 5,

        /// <summary>
        /// Indicates reexecution of an action if all the actions so far have been unsuccessful.
        /// This applies to mostly just the EnemyAlgo.
        /// The player is always desperate.
        /// This means that for example they would always bump againt the wall if the movement fails.
        /// This is still a TODO.
        /// The easiest way of making it work as expected is to check if the action had moving, and bump if it did.
        /// </summary>
        IsDesperate                 = 1 << 6,
    };
    
    public partial class Acting : IBehavior
    {
        [Chain("Do")]               private readonly Chain<Context> _DoChain;        
        [Chain("Check")]            private readonly Chain<Context> _CheckChain;
        [Chain("SubstituteAction")] private readonly Chain<SubstitutionContext> _SubstituteActionChain;
        [Chain("Success")]          private readonly Chain<Context> _SuccessChain;
        [Chain("Fail")]             private readonly Chain<Context> _FailChain;

        [Inject] public readonly System.Func<Entity, CompiledAction> ActionCalculationAlgorithm;
        [Inject] public readonly System.Action<Context> ActionExecutionAlgorithm;
        [Inject] public readonly Order order;

        public ActingState _flags;
        public CompiledAction _nextAction;
        public Entity actor;

        public class SubstitutionContext : ContextBase
        {
            public Entity actor => acting.actor;
            public readonly Acting acting;
            public readonly CompiledAction initialAction;
            public CompiledAction currentAction;

            public bool HasActionBeenReset => initialAction != currentAction;

            public SubstitutionContext(Acting acting, CompiledAction action)
            {
                this.acting = acting;
                this.initialAction = action;
                this.currentAction = action;
            }

            public void SetAction(IAction action)
            {
                this.currentAction = this.currentAction.WithAction(action);
            }

            public void SetAction(in CompiledAction action)
            {
                this.currentAction = action;
            }
        }

        public class Context : IPropagating
        {
            public Entity actor => acting.actor;
            public Acting acting;
            public CompiledAction action;

            // yay, two bools within 32 bits. (this is probably not very useful, but I don't care :D)
            public AnonymousInt32Flags _aflags;
            public bool Propagate { get => _aflags.Get(0); set => _aflags.Set(0, value); }
            public bool Success   { get => _aflags.Get(1); set => _aflags.Set(1, value); }

            public Context(Acting acting)
            {
                this.acting = acting;
                this.action = acting.GetNextAction();
                this.Propagate = true;
            }
        }


        [Alias("InitActing")]
        public Acting Init(Entity actor)
        {
            this.actor = actor;
            return this;
        }

        public bool ActivateWith(CompiledAction action)
        {
            SetPotentialAction(action);
            return Activate();
        }

        public void ActivateIfDidNotAct()
        {
            if (!_flags.HasFlag(ActingState.DidAction))
            {
                Activate();
            }
        }

        /// <summary>
        /// In essense, executes the stored calculated (or set directly) action.
        /// </summary>
        public bool Activate()
        {
            var context = new Context(this);

            _flags |= ActingState.DoingAction;

            if (_CheckChain.PassWithPropagationChecking(context))
            {
                // Let's move it here for now
                if (!context.action.HasAction())
                {
                    _flags = ActingState.DidAction | ActingState.ActionSucceeded;
                    _SuccessChain.Pass(context);
                    return true;
                }

                context.Success = true;
                ActionExecutionAlgorithm(context);
            }

            _flags = _flags.Set(ActingState.ActionSucceeded, context.Success);

            if (context.Success)
                _SuccessChain.Pass(context);
            else
                _FailChain.Pass(context);

            _flags = _flags.Set(ActingState.DidAction)
                           .Unset(ActingState.DoingAction);

            return context.Success;
        }

        /// <summary>
        /// Resets the flags representing the internal state. 
        /// The flags control interaction between entities acting in the world. 
        /// If the Activate() is to be called manually, it would still work, even without resetting beforehand.
        /// </summary>
        [Export(Chain = "Ticking.Do", Dynamic = true)] 
        public void Reset()
        {
            _flags = 0;
        }

        /// <summary>
        /// Returns true if the entity could act given the current order of the global world.
        /// </summary>
        [Alias("IsCurrentOrderFavorable")]
        public bool IsCurrentOrderFavorable()
        {
            return (int) World.Global.State.currentPhase == (int) order;
        }

        /// <summary>
        /// Runs the action selection algorithm if the action has not been set already.
        /// Note that the actual action that will be selected is affected by the ActionSelected chain.
        /// See <see cref="SetPotentialAction"/> for more details.
        /// </summary>
        public void CalculateAndSetAction()
        {
            // TODO: add a flag "action selected"
            if (!_flags.HasFlag(ActingState.ActionSet) && ActionCalculationAlgorithm != null)
            {
                SetPotentialAction(ActionCalculationAlgorithm(actor));
            }
        }

        /// <summary>
        /// Tries setting the next action to the corresponding value.
        /// The given action may be altered, depending on the current state of the entity.
        /// For example, if the entity is sliding, the sliding action will be substituted here.
        /// The action's direction may also be influenced by e.g. dizzying effects.
        /// This action substitution is governed by the ActionSelected chain.
        /// </summary>
        public void SetPotentialAction(CompiledAction action)
        {
            _nextAction = action;
            _flags = _flags
                .Set(ActingState.ActionSet)
                // GetAction() should retraverse the chain
                .Unset(ActingState.ActionSubstitutionTraversed);
        }

        /// <summary>
        /// Returns the next action.
        /// This ensures the action has been modified by handlers in the ActionSelected chain.
        ///
        /// TODO: 
        /// This would fail, if the chain were to be changed by some code adding a handler to it.
        /// Then that handler may happen to be changing the action to something completely different.
        /// However, this would only happen if the action is to be computed via this function prior to 
        /// the Acting behavior being activated.
        /// So, the solution in this case would be to change the type of chain we have here.
        /// We would like to have this chain mark itself dirty when a handler is added.
        /// Another solution would be to add handlers to this chain only via a function on Acting.
        /// However, currently, the latter is completely unfeasible.
        /// The first one should become possible when I poke the code generator some more.
        ///
        /// The only possible sort of workarounds currenly would be to like see the count of handlers 
        /// in the chain each time, or compare hashes of all handlers or something like that, but that
        /// won't work in all cases and that's a garbage solution as a whole.
        ///
        /// </summary>
        public CompiledAction GetNextAction()
        {
            if (_flags.HasFlag(ActingState.ActionSubstitutionTraversed))
            {
                return _nextAction;
            }
            
            var context = new SubstitutionContext(this, _nextAction);

            // We modify it here and not in Activate() because this matters for e.g. predictions.
            // E.g. if the entity is sliding, then the attacking cannot happen.
            _SubstituteActionChain.PassWithPropagationChecking(context);

            // If the action gets substituted, the ActionSubstituted set the sustituted flag.
            _flags = _flags
                .Set(ActingState.ActionSubstituted, context.HasActionBeenReset)
                // No need to traverse it again if the next action is needed
                .Set(ActingState.ActionSubstitutionTraversed);

            _nextAction = context.currentAction;

            return _nextAction;
        }
        
        /// <summary>
        /// Use in the <c>InitComponents()</c> of your entity type.
        /// </summary>
        public void DefaultPreset(Entity subject)
        {
            ResetHandlerWrapper.HookTo(subject);
        }

        /// <summary>
        /// Returns the directions that the entity would try while trying their action.
        /// </summary>
        public IEnumerable<IntVector2> GetPossibleDirections()
        {
            // This will have to be patched, if any other multidirectional algos appear
            // that depend on something else than the movs function.
            if (_nextAction.HasAction())
            {
                if (actor.TryGetSequential(out var sequential))
                {
                    var currentStep = sequential.sequence.CurrentStep;
                    if (currentStep.movs != null)
                    {
                        foreach (var dir in currentStep.movs(actor.GetTransform()))
                        {
                            yield return dir;
                        }
                    }
                }
                else
                {
                    yield return ((CompiledAction)_nextAction).direction;
                }
            }
        }
    }
}