using System.Collections.Generic;
using System.Numerics;

namespace Core
{
    public class Sequence
    {
        public StepData[] steps;

        int currentStepIndex = 0;
        int currentRepeatCount = 0;
        public Action CurrentAction
        {
            get
            {
                return steps[currentStepIndex].action.Copy();
            }
        }

        public void TickAction(Entity entity)
        {
            currentRepeatCount++;
            StepData currentStep = steps[currentStepIndex];

            if (!currentStep.IsRepeatLimitMet(currentRepeatCount))
                return;

            int relativeIndex = currentStep.CheckSuccessAndGetRelativeIndex(entity);

            if (relativeIndex != 0)
            {
                currentStepIndex += relativeIndex + steps.Length;
                currentStepIndex %= steps.Length;
                currentRepeatCount = 0;
                currentStep.Exit(entity);
                steps[currentStepIndex].Enter(entity);
            }
        }

        public List<Vector2> GetMovs(Entity e, Action a)
        {
            return steps[currentStepIndex].movs(e, a);
        }
    }
}