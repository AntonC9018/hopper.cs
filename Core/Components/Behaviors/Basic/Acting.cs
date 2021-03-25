using Hopper.Core.Chains;
using Hopper.Utils.Chains;
using Hopper.Utils.Vector;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hopper.Core.Components.Basic
{
    [Chains("Do", "Check", "Success", "Fail")]
    public class Acting : Behavior, IBehavior
    {
        [Flags] public enum Flags 
        {
            DidAction = 0b_001,
            DoingAction = 0b_010,
            ActionSucceeded = 0b_100
        };

        [Inject] public System.Func<Entity, ParticularAction> _CalculateAction;
        [Inject] public System.Action<Context> _DoAction;
        public ParticularAction nextAction;

        public class Context : ActorEvent
        {
            public ParticularAction action;
            public bool success = false;
        }

        [Alias("Act")] 
        public bool Activate()
        {
            var ctx = new Context
            {
                actor = m_entity,
                action = nextAction
            };

            if (nextAction == null)
            {
                _flags = Flags.DidAction | Flags.ActionSucceeded;
                TraverseSuccess(ctx);
                return true;
            }

            SetDoingAction();

            if (TraverseCheck(ctx))
            {
                ctx.success = true;
                _DoAction(ctx);
            }

            ctx.propagate = true;
            SetActionSucceed(ctx.success);

            if (ctx.success)
                TraverseSuccess(ctx);
            else
                TraverseFail(ctx);

            SetDoneAction();
            SetDoingAction(false);

            return ctx.success;
        }

        public void CalculateNextAction()
        {
            if (nextAction == null && _CalculateAction != null)
            {
                nextAction = _CalculateAction(m_entity);
            }
        }

        public IEnumerable<IntVector2> GetPossibleDirections()
        {
            // This will have to be patched, if any other multidirectional algos appear
            // that depend on something else than the movs function.
            if (nextAction != null)
            {
                if (m_entity.Behaviors.Has<Sequential>())
                {
                    var currentStep = m_entity.Behaviors.Get<Sequential>().Sequence.CurrentStep;
                    if (currentStep.movs != null)
                    {
                        foreach (var dir in currentStep.movs(m_entity))
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