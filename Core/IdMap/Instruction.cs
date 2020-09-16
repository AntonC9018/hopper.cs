namespace Core
{
    public class MapInstruction
    {
        public string modName;
        public int listIndex;

        public MapInstruction(string modName, int listIndex)
        {
            this.modName = modName;
            this.listIndex = listIndex;
        }
    }
}