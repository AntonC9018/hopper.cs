using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Hopper.Core;
using Hopper.Core.ActingNS;
using Hopper.Core.Components;
using Hopper.Core.Components.Basic;
using Hopper.Core.Items;
using Hopper.Core.Retouchers;
using Hopper.Core.Stat;
using Hopper.Core.Targeting;
using Hopper.Core.WorldNS;
using Hopper.TestContent;
using Hopper.TestContent.BindingNS;
using Hopper.TestContent.PinningNS;
using Hopper.TestContent.SimpleMobs;
using Hopper.Utils;
using Hopper.Utils.Vector;
using static Hopper.Utils.Vector.IntVector2;

namespace Hopper.Mine
{

    public class Program
    {
        class Inheriting : List<int>
        {
        }

        class Forwarding : IList<int>
        {
            public readonly List<int> list = new List<int>();

            public int this[int index] { get => list[index]; set => list[index] = value; }
            public int Count => list.Count;
            public bool IsReadOnly => ((ICollection<int>)list).IsReadOnly;
            public void Add(int item) => list.Add(item);
            public void Clear() => list.Clear();
            public bool Contains(int item) => list.Contains(item);
            public void CopyTo(int[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);

            public IEnumerator<int> GetEnumerator() => list.GetEnumerator();
            public int IndexOf(int item) => list.IndexOf(item);
            public void Insert(int index, int item) => list.Insert(index, item);
            public bool Remove(int item) => list.Remove(item);
            public void RemoveAt(int index) => list.RemoveAt(index);

            IEnumerator IEnumerable.GetEnumerator() => list.GetEnumerator();
        }

        public static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
 
            stopwatch.Start();
            for (int i = 0; i < 10; i++)
                Forwardings();
            stopwatch.Stop();
 
            Console.WriteLine($"Elapsed Time is {stopwatch.ElapsedMilliseconds} ms");
            Console.WriteLine($"Medium time is {stopwatch.ElapsedMilliseconds / 10} ms");
        }

        public static void Forwardings()
        {
            List<Forwarding> forwardings = new List<Forwarding>();
            for (int i = 0; i < 1000; i++)
            {
                forwardings.Add(new Forwarding());

                for (int j = 0; j < 1000; j++)
                {
                    forwardings[i].Add(j * i);
                }
            }

            int sum = 0;
            for (int k = 0; k < 20; k++)
            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    sum += forwardings[i][j];
                }
            }

            Console.WriteLine(sum);
        }

        public static void Inheritings()
        {
            List<Inheriting> inheritings = new List<Inheriting>();
            for (int i = 0; i < 1000; i++)
            {
                inheritings.Add(new Inheriting());

                for (int j = 0; j < 1000; j++)
                {
                    inheritings[i].Add(j * i);
                }
            }

            int sum = 0;
            for (int k = 0; k < 20; k++)
            for (int i = 0; i < 1000; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    sum += inheritings[i][j];
                }
            }
            Console.WriteLine(sum);

        }
    }
}