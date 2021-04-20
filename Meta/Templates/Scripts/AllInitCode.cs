using System.Collections.Generic;
using Hopper.Meta;
using Hopper.Meta.Stats;

namespace Hopper.Meta.Template
{
    public partial class AllInitCode
    {
        public string Namespace;
        public IEnumerable<ComponentSymbolWrapper> components;
        public IEnumerable<BehaviorSymbolWrapper> behaviors;
        public IEnumerable<StaticClassSymbolWrapper> staticClasses;
        public IEnumerable<EntityTypeWrapper> entityTypes;
        public Scope<StatType> statRootScope;
    }
}