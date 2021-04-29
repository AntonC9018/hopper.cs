using Hopper.Utils.Vector;
using System.Collections.Generic;
using Hopper.Shared.Attributes;
using Hopper.Utils.Chains;

namespace Hopper.Core.Components.Basic
{
    [Chains("Do", "Check", "Success", "Fail")]
    [NoActivation]
    public partial class Acting : IBehavior
    {
        [Flags] public enum Flags 
        {
            DidAction = 0b_001,
            DoingAction = 0b_010,
            ActionSucceeded = 0b_100
        };

        [Flags] public Flags _flags;

        [Inject] public System.Func<Entity, ParticularAction> ActionCalculationAlgorithm;
        [Inject] public System.Action<Context> ActionExecutionAlgorithm;
        [Inject] public Order order;
        public ParticularAction nextAction;
        public Entity actor;

        public class Context : ContextBase
        {
            public Entity actor => acting.actor;
            [Omit] public Acting acting;
            [Omit] public ParticularAction action;
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

            if (nextAction == null)
            {
                _flags = Flags.DidAction | Flags.ActionSucceeded;
                TraverseSuccess(ctx);
                return true;
            }

            _flags |= Flags.DoingAction;

            if (TraverseCheck(ctx))
            {
                ctx.success = true;
                ActionExecutionAlgorithm(ctx);
            }

            ctx.propagate = true;

            if (ctx.success) _flags |=  Flags.ActionSucceeded;
            else             _flags &= ~Flags.ActionSucceeded;

            if (ctx.success)
                TraverseSuccess(ctx);
            else
                TraverseFail(ctx);

            _flags |=  Flags.DidAction;
            _flags &= ~Flags.DoingAction;

            return ctx.success;
        }

        [Export(Chain = "Ticking.Do")] 
        public void ResetAction()
        {
            _flags = 0;
            nextAction = null;
        }

        [Alias("IsCurrentOrderFavorable")]
        public bool IsCurrentOrderFavorable()
        {
            return World.Global.state.currentPhase == order;
        }

        public void CalculateNextAction()
        {
            if (nextAction == null && ActionCalculationAlgorithm != null)
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
            if (nextAction != null)
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
                    yield return ((ParticularDirectedAction)nextAction).direction;
                }
            }
            yield break;
        }
    }
}