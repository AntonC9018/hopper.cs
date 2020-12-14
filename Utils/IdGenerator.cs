namespace Hopper.Utils
{
    // USAGE:
    /*
        static IdGenerator s_idGenerator = new IdGenerator();
        public readonly int id = s_idGenerator.GetNextId();
    */
    public class IdGenerator
    {
        private int id;
        public int Count => id;
        public int GetNextId()
        {
            return ++id;
        }
        public void Reset()
        {
            id = 0;
        }
    }
}