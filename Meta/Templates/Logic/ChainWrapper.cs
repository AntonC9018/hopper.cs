using Microsoft.CodeAnalysis;
using System.Linq;

namespace Meta
{
    public class ChainWrapper
    {
        public string Name;
        public bool ShouldGenerateParamsMethod; 
        public bool ShouldGenerateTraverseMethod;

        public ChainWrapper(string name, INamedTypeSymbol parentSymbol)
        {
            Name = name;
            ShouldGenerateParamsMethod = parentSymbol.GetMembers().OfType<IMethodSymbol>().Any(m => m.Name == name);;
            ShouldGenerateTraverseMethod = parentSymbol.GetMembers().OfType<IMethodSymbol>().Any(m => m.Name == $"Traverse{name}");;
        }
    }
}