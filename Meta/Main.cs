using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Locator;

namespace Hopper.Meta
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            MSBuildLocator.RegisterDefaults();
            await Start(ParseArguments(args));
        }

        public static IEnumerable<ModProject> ParseArguments(string[] args)
        {
            // let's say, it's just AssemblyName[;ProjectPath] for now
            foreach (var term in args)
            {
                var data = term.Split(';');
                if (data.Length == 1)
                {
                    yield return new ModProject(data[0]);
                }
                else if (data.Length == 2)
                {
                    yield return new ModProject(data[0], data[1]);
                }
                else
                {
                    Console.WriteLine($"Invalid argument: {term}, skipping.");
                }
            }
        }

        public static async Task Start(IEnumerable<ModProject> things)
        {
            var generator = new Generator();
            await generator.Start(things);
        }
    }
}