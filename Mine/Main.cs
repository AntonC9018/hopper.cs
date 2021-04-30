using System;
using System.Reflection;
using System.Text;

namespace Hopper.Mine
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Assembly lib = typeof(Hopper.Core.Action).Assembly;
            Type[] types;
            try
            {
                types = lib.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Exception exSub in ex.LoaderExceptions)
                {
                    Console.WriteLine(exSub.Message);
                }
                types = ex.Types;
            }
            
            foreach (Type type in types)
            {
                if (type != null)
                    Console.WriteLine(type.FullName);
            }
        }
    }
}