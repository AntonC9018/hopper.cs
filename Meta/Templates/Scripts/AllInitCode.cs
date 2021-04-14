using System.Collections.Generic;
using Meta;

namespace Hopper.Meta.Template
{
    public partial class AllInitCode
    {
        public string Namespace;
        public IEnumerable<ComponentSymbolWrapper> components;
        public IEnumerable<BehaviorSymbolWrapper> behaviors;
        public IEnumerable<StaticClassSymbolWrapper> staticClasses;
    }
}