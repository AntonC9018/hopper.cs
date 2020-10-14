using System.Collections.Generic;
using Core.Behaviors;
using Utils.Vector;

namespace Core
{
    public delegate void InsideChangeFunction(Entity entity);
    public delegate Result SuccessCheckFunction(Entity entity);

    public class Result
    {
        public int? index = null;
        public bool success = false;
    }

    public class Step
    {
        public int relativeStepIndexSuccess = 1;
        public int relativeStepIndexFail = 0;

        public SuccessCheckFunction successFunction = null;
        public int repeat = 1;

        // an clonable instance of Action
        public Action action = null;
        public InsideChangeFunction enterFunction = null;
        public InsideChangeFunction exitFunction = null;
        public MovsFunc movs = null;

        public int CheckSuccessAndGetRelativeIndex(Entity e)
        {

            if (successFunction != null)
            {
                var result = successFunction(e);
                if (result.index != null)
                {
                    return (int)result.index;
                }
                if (result.success)
                {
                    return relativeStepIndexSuccess;
                }
                return relativeStepIndexFail;
            }

            var acting = e.Behaviors.Get<Acting>();

            if (acting != null)
            {
                if (acting.b_didActionSucceed)
                {
                    return relativeStepIndexSuccess;
                }
                return relativeStepIndexFail;
            }

            throw new System.Exception("Sequence expects Acting decorator unless a custom check success function is specified for each of the steps");

        }

        public bool IsRepeatLimitMet(int currentRepeatCount)
        {
            return currentRepeatCount >= repeat;
        }

        public void Enter(Entity e)
        {
            if (enterFunction != null)
                enterFunction(e);
        }

        public void Exit(Entity e)
        {
            if (exitFunction != null)
                exitFunction(e);
        }

        public List<IntVector2> GetMovs(Entity e)
        {
            return movs(e, action);
        }
    }
}