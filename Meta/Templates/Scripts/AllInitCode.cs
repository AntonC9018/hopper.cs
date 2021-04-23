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
        public IEnumerable<ExportedMethodsClassSymbolWrapper> methodClasses;
        public IEnumerable<FieldSymbolWrapper> methodClassInstances;
        public IEnumerable<FieldSymbolWrapper> fieldsRequiringInit;
        public IEnumerable<EntityTypeWrapper> entityTypes;
        public IEnumerable<FieldSymbolWrapper> slots;
        public Scope<StatType> statRootScope;
    }
}