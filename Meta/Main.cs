using System.IO;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Hopper.Meta.Template;
using System.Collections.Immutable;

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