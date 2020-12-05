namespace Hopper.Utils
{
    // USAGE:
    /*
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();
    */
    public class IdGenerator
    {
        private int id = 0;
        public int GetNextId()
        {
            return id++;
        }
    }
}