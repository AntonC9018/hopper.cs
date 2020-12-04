using Core.Stats;

namespace Core
{
    public class StatusFile : StatFile
    {
        // how many ticks will it last
        public int amount = 0;
        // the minimum amount of resistance to not let the status apply
        public int power = 0;
    }
}