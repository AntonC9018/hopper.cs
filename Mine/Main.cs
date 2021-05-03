using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Items;
using Hopper.Core.Predictions;
using Hopper.Core.Retouchers;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.TestContent;
using Hopper.TestContent.SimpleMobs;
using Hopper.Utils;
using Hopper.Utils.Vector;

namespace Hopper.Mine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Hopper.Core.Main.Init();
            Hopper.TestContent.Main.Init();
            var id_foo = new Identifier(0, 1);
            var id_bar = new Identifier(0, 2);

            var subpool = new SubPool{
                { id_foo, 1, 1 }, 
                { id_bar, 1, 1 }
            };

            // Now, we cannot predict in which order the items got placed in the
            // dictionary, since the dictionary is not sorted.
            // So lets just test that the items were both taken out
            var id  = subpool.Draw(0.1);
            subpool.AdjustAmount(id, -1);
            var id1 = subpool.Draw(0.1);
            subpool.AdjustAmount(id1, -1);
        }
    }
}