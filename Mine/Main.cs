using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Hopper.Mine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            System.Console.WriteLine("Hello");

            try
            {
                Assembly lib = typeof(Hopper.Core.Stat.Attack).Assembly;
                foreach (Type type in lib.GetTypes())
                {
                    Console.WriteLine(type.FullName);
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    Console.WriteLine(exSub.Message);
                }
            }
        }
    }
}