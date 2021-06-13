
using System.Collections.Generic;
using Hopper.Core;
using Hopper.Core.Components.Basic;
using Hopper.Core.Mods;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.Core.WorldNS;
using Hopper.TestContent.BindingNS;
using Hopper.Utils.Vector;

namespace Mine
{
    public class Program
    {
        public static IEnumerable<int> Try()
        {
            System.Console.WriteLine("Before 1");
            yield return 1;
            System.Console.WriteLine("After 1");

            System.Console.WriteLine("Before 2");
            yield return 2;
            System.Console.WriteLine("After 2");


        }
        public static void Main()
        {
            var e = Try();
            foreach (var i in e) {}
            // var en = e.GetEnumerator();

            // while (en.MoveNext())
            // {
            //     var val = en.Current;
            // }
        }
    }
}