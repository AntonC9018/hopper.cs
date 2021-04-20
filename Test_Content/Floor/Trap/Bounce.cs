using Hopper.Core.Stat;
using Hopper.Core.Stat.Basic;

namespace Hopper.Test_Content.Floor
{
    public class Bounce : StatFile
    {
        public int power;
        public static Push.Source Source = new Push.Source { resistance = 1 };
    }
}