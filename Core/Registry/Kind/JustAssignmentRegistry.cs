using Hopper.Utils;

namespace Hopper.Core.Registry
{
    public class JustAssignmentRegistry
    {
        private IdGenerator generator;
        public int Count => generator.Count;

        public int GetNextId()
        {
            return generator.GetNextId();
        }
    }
}