using System.Linq;
using Microsoft.CodeAnalysis;

namespace Hopper.Meta
{
    public class ExportedMethodsClassSymbolWrapper : TypeSymbolWrapperBase
    {
        public ExportedMethodSymbolWrapper[] exportedMethods;

        public ExportedMethodsClassSymbolWrapper(INamedTypeSymbol symbol) : base(symbol)
        {
        }

        protected override bool Init(GenerationEnvironment env)
        {
            if (!base.Init(env)) return false;

            exportedMethods = GetNonNativeExportedMethods(env).ToArray();

            // If there are no exported methods, this class is unusable anyway
            // and should be either given another symbol, or thrown away.
            return (exportedMethods.Length > 0) ? base.Init(env) : false;
        }

        public override string TypeText => "Static Class";
    }
}