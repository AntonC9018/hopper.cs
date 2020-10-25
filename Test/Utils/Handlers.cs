using Core.Behaviors;

namespace Test.Utils
{
    public static class Handlers
    {
        public static void StopMove(Moving.Event ev)
        {
            ev.propagate = false;
        }
    }
}