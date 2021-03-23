using System;

namespace Code
{
    class Program
    {
        public class AliasAttribute : Attribute
        {
            public AliasAttribute(params string[] aliases)
            {
                foreach (var alias in aliases)
                    Console.WriteLine(aliases);
            }
        }

        [Alias("Test1", "Test2")]
        public static void Thing(int i)
        {

        }
    }
}