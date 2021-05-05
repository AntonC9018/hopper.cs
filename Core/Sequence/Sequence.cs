using System.Runtime.Serialization;
using Hopper.Core.Components.Basic;
using Hopper.Utils;
using Hopper.Utils.Vector;

namespace Hopper.Core
{
    public struct Sequence
    {
        private Step[] m_steps;

        public Sequence(params Step[] steps)
        {
            Assert.AreNotEqual(0, steps.Length, "The step data must include at least one step");
            this.m_steps = steps;
            this.currentStepIndex = 0;
            this.currentRepeatCount = 0;
        }

        private int currentStepIndex;
        private int currentRepeatCount;

        public CompiledAction CurrentAction
        {
            get
            {
                // This will work even if the action is null (it is an extension method)
                return m_steps[currentStepIndex].action.Compile(IntVector2.Zero);
            }
        }

        public Step CurrentStep => m_steps[currentStepIndex];

        public void TickAction(Acting acting)
        {
            currentRepeatCount++;
            Step currentStep = m_steps[currentStepIndex];

            if (!currentStep.IsRepeatLimitMet(currentRepeatCount))
                return;

            int relativeIndex = currentStep.CheckSuccessAndGetRelativeIndex(acting);

            if (relativeIndex != 0)
            {
                currentStepIndex += relativeIndex + m_steps.Length;
                currentStepIndex %= m_steps.Length;
                currentRepeatCount = 0;
                currentStep.Exit(acting);
                m_steps[currentStepIndex].Enter(acting);
            }
        }
    }
}