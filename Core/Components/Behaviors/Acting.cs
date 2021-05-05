using Hopper.Utils.Vector;
using System.Collections.Generic;
using Hopper.Shared.Attributes;
using Hopper.Utils.Chains;
using Hopper.Utils;

namespace Hopper.Core.Components.Basic
{
    [Flags] public enum ActingState 
    {
        DidAction = 0b_001,
        DoingAction = 0b_010,
        ActionSucceeded = 0b_100
    };
    
    [Chains("Do", "Check", "Success", "Fail")]
    [NoActivation]
    public partial class Acting : IBehavior
    {
        [Flags] public ActingState _flags;

        [Inject] public readonly System.Func<Entity, CompiledAction> ActionCalculationAlgorithm;
        [Inject] public readonly System.Action<Context> ActionExecutionAlgorithm;
        [Inject] public readonly Order order;
        public CompiledAction nextAction;
        public IntVector2 selectedDirection;
        public Entity actor;

        public class Context : ContextBase
        {
            public Entity actor => acting.actor;
            [Omit] public Acting acting;
            [Omit] public CompiledAction action;
            [Omit] public bool success = false;
        }


        [Alias("InitActing")]
        public Acting Init(Entity actor)
        {
            this.actor = actor;
            return this;
        }

        public bool Activate()
        {
            var ctx = new Context
            {
                acting = this,
                action = nextAction
            };

            _flags |= ActingState.DoingAction;

            if (TraverseCheck(ctx))
            {
                // Let's move it here for now
                if (!ctx.action.HasAction())
                {
                    _flags = ActingState.DidAction | ActingState.ActionSucceeded;
                    TraverseSuccess(ctx);
                    return true;
                }

                ctx.success = true;
                ActionExecutionAlgorithm(ctx);
            }

            ctx.propagate = true;

            if (ctx.success) _flags |=  ActingState.ActionSucceeded;
            else             _flags &= ~ActingState.ActionSucceeded;

            if (ctx.success)
                TraverseSuccess(ctx);
            else
                TraverseFail(ctx);

            _flags |=  ActingState.DidAction;
            _flags &= ~ActingState.DoingAction;

            return ctx.success;
        }

        [Export(Chain = "Ticking.Do")] 
        public void ResetAction()
        {
            _flags = 0;
            nextAction = new CompiledAction();
        }

        [Alias("IsCurrentOrderFavorable")]
        public bool IsCurrentOrderFavorable()
        {
            return World.Global.state.currentPhase == order;
        }

        public void CalculateNextAction()
        {
            if (!nextAction.HasAction() && ActionCalculationAlgorithm != null)
            {
                nextAction = ActionCalculationAlgorithm(actor);
            }
        }

        public void DefaultPreset(Entity subject)
        {
            subject.GetTicking()._DoChain.Add(ResetActionHandler);
        }

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