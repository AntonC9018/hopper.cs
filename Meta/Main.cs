using System.Threading.Tasks;
using Microsoft.Build.Locator;

namespace Meta
{
    class Program
    {
        public static async Task Main()
        {
            MSBuildLocator.RegisterDefaults();
            var generator = new Generator();
            await generator.Start();
        }
    }
}