namespace Core
{
    public class IdGenerator
    {
        private int id = 0;
        public int GetNextId()
        {
            return id++;
        }
    }
}