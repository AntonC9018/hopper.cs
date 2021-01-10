using Hopper.Core.Stats;
using Hopper.Core.Stats.Basic;

namespace Hopper.Test_Content.Floor
{
    public class Bounce : StatFile
    {
        public int power;
        public static Push.Source Source = new Push.Source { resistance = 1 };
    }
}