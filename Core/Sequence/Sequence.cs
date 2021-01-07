using System.Runtime.Serialization;
using Hopper.Utils;

namespace Hopper.Core
{
    [DataContract]
    public class Sequence
    {
        private Step[] m_steps;

        public Sequence(params Step[] steps)
        {
            Assert.AreNotEqual(0, steps.Length, "The step data must include at least one step");
            this.m_steps = steps;
        }

        [DataMember] private int currentStepIndex = 0;
        [DataMember] private int currentRepeatCount = 0;

        public ParticularAction CurrentAction
        {
            get
            {
                return m_steps[currentStepIndex].action?.ToParticular();
            }
        }

        public Step CurrentStep => m_steps[currentStepIndex];

        public void TickAction(Entity actor)
        {
            currentRepeatCount++;
            Step currentStep = m_steps[currentStepIndex];

            if (!currentStep.IsRepeatLimitMet(currentRepeatCount))
                return;

            int relativeIndex = currentStep.CheckSuccessAndGetRelativeIndex(actor);

            if (relativeIndex != 0)
            {
                currentStepIndex += relativeIndex + m_steps.Length;
                currentStepIndex %= m_steps.Length;
                currentRepeatCount = 0;
                currentStep.Exit(actor);
                m_steps[currentStepIndex].Enter(actor);
            }
        }
    }
}