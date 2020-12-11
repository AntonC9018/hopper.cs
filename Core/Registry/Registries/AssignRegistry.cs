using Hopper.Utils;

namespace Hopper.Core
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