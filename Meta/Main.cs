using System.Threading.Tasks;
using Hopper.Meta.Stats;
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
                "../Core/Hopper_Core.csproj", 
                "../TestContent/Hopper_Test_Content.csproj" 
                });
        }
    }
}