using System.Threading.Tasks;
using Microsoft.Build.Locator;

namespace Hopper.Meta
{
    class Program
    {
        public static async Task Main()
        {
            MSBuildLocator.RegisterDefaults();
            var generator = new Generator();
            await generator.Start(new string[] { 
                "../Core/Hopper.Core.csproj", 
                "../TestContent/Hopper.TestContent.csproj" 
            });
        }
    }
}