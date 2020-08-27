using System.Collections.Generic;
using Vector;

namespace Core
{
    public class Sequence
    {
        public StepData[] stepData;
        public Entity actor;

        int currentStepIndex = 0;
        int currentRepeatCount = 0;
        public Action CurrentAction
        {
            get
            {
                return stepData[currentStepIndex].action?.Copy();
            }
        }

        public void TickAction()
        {
            currentRepeatCount++;
            StepData currentStep = stepData[currentStepIndex];

            if (!currentStep.IsRepeatLimitMet(currentRepeatCount))
                return;

            int relativeIndex = currentStep.CheckSuccessAndGetRelativeIndex(actor);

            if (relativeIndex != 0)
            {
                currentStepIndex += relativeIndex + stepData.Length;
                currentStepIndex %= stepData.Length;
                currentRepeatCount = 0;
                currentStep.Exit(actor);
                stepData[currentStepIndex].Enter(actor);
            }
        }

        public List<IntVector2> GetMovs()
        {
            return stepData[currentStepIndex].GetMovs(actor);
        }
    }
}