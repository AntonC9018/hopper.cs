using Hopper.Utils.Vector;
using System.Collections.Generic;
using Hopper.Shared.Attributes;

namespace Hopper.Core.Components.Basic
{
    [Chains("Do", "Check", "Success", "Fail")]
    [ActivationAlias("Act")]
    public partial class Acting : IBehavior
    {
        [Flags] public enum Flags 
        {
            DidAction = 0b_001,
            DoingAction = 0b_010,
            ActionSucceeded = 0b_100
        };

        [Flags] public Flags _flags;

        [Inject] public System.Func<Entity, ParticularAction> _CalculateAction;
        [Inject] public System.Action<Context> _DoAction;
        public ParticularAction nextAction;

        public class Context : ActorContext
        {
            [Omit] public ParticularAction action;
            [Omit] public bool success = false;
        }

        public bool Activate(Entity entity)
        {
            var ctx = new Context
            {
                actor = entity,
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
                _DoAction(ctx);
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

        [Export(Chain = "TickBehavior.Do")] 
        public void ResetAction()
        {
            _flags = 0;
            nextAction = null;
        }

        public void CalculateNextAction(Entity entity)
        {
            if (nextAction == null && _CalculateAction != null)
            {
                nextAction = _CalculateAction(entity);
            }
        }

        public void DefaultPreset(EntityFactory factory)
        {
            factory.GetComponent(TickBehavior.Index)._DoChain.Add(ResetActionHandler);
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