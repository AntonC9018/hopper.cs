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
        DidAction         = 1,
        DoingAction       = 2,
        ActionSucceeded   = 4,
        ActionSet         = 8,
        ActionSubstituted = 16
    };
    
    public partial class Acting : IBehavior
    {
        [Chain("Do")]             private readonly Chain<Context> _DoChain;        
        [Chain("Check")]          private readonly Chain<Context> _CheckChain;
        [Chain("ActionSelected")] private readonly Chain<Context> _ActionSelectedChain;
        [Chain("Success")]        private readonly Chain<Context> _SuccessChain;
        [Chain("Fail")]           private readonly Chain<Context> _FailChain;

        [Inject] public readonly System.Func<Entity, CompiledAction> ActionCalculationAlgorithm;
        [Inject] public readonly System.Action<Context> ActionExecutionAlgorithm;
        [Inject] public readonly Order order;

        public ActingState _flags;
        public CompiledAction nextAction;
        public Entity actor;

        public class Context : ContextBase
        {
            public Entity actor => acting.actor;
            [Omit] public Acting acting;
            [Omit] public CompiledAction action;
            [Omit] public bool success = false;

            public bool HasActionBeenReset => acting._flags.HasFlag(ActingState.ActionSubstituted);

            public void SetAction(in IAction action)
            {
                this.action = this.action.WithAction(action);
                acting._flags |= ActingState.ActionSubstituted;
            }

            public void SetAction(in CompiledAction action)
            {
                this.action = action;
                acting._flags |= ActingState.ActionSubstituted;
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
            this.nextAction = action;
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
            var ctx = new Context
            {
                acting = this,
                action = nextAction
            };

            _flags |= ActingState.DoingAction;

            if (_CheckChain.PassWithPropagationChecking(ctx))
            {
                // Let's move it here for now
                if (!ctx.action.HasAction())
                {
                    _flags = ActingState.DidAction | ActingState.ActionSucceeded;
                    _SuccessChain.Pass(ctx);
                    return true;
                }

                ctx.success = true;
                ActionExecutionAlgorithm(ctx);
            }

            _flags = _flags.Set(ActingState.ActionSucceeded, ctx.success);

            if (ctx.success)
                _SuccessChain.Pass(ctx);
            else
                _FailChain.Pass(ctx);

            _flags = _flags.Set(ActingState.DidAction)
                           .Unset(ActingState.DoingAction);

            return ctx.success;
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
        /// Note that the actual action that will be selected is affected by the  ActionSelected chain (unimplemented).
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
        /// This action substitution is governed by the ActionSelected chain (unimplemented).
        /// </summary>
        public void SetPotentialAction(CompiledAction action)
        {
            // Iterate a chain to maybe change the action.
            // TODO: 
            // This should work with a different context type.
            // This one has fields only useful for execution, and vice-versa.
            var context = new Context { action = action, acting = this };

            // If the action gets substituted, the ActionSubstituted flag on acting gets set.
            _ActionSelectedChain.PassWithPropagationChecking(context);
            
            // Save the modified action.
            // TODO: 
            // This seems wasteful. 
            // What is the point of storing this in the context, when it could be set directly on the acting?
            // Would it be ok to save it directly?
            nextAction = context.action;
            _flags |= ActingState.ActionSet;
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
            if (nextAction.HasAction())
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
                    yield return ((CompiledAction)nextAction).direction;
                }
            }
        }
    }
}