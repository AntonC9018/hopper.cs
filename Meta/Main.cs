using System.Threading.Tasks;
using Microsoft.Build.Locator;

namespace Hopper.Meta
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            MSBuildLocator.RegisterDefaults();
            var generator = new Generator();
            await generator.Start(args);
        }
    }
}