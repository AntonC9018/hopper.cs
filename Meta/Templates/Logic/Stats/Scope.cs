using System.Collections.Generic;
using System.Linq;

namespace Hopper.Meta.Stats
{
    public class Scope<T>
    {
        public Dictionary<string, Scope<T>> children; 
        public Scope<T> parentScope;
        public T value; // may have a stat defined here

        public Scope(T value, Scope<T> parentScope = null)
        {
            this.parentScope = parentScope;
            this.value = value;
            this.children = new Dictionary<string, Scope<T>>();
        }

        public bool IsRoot => parentScope == null;

        public void Add(string name, Scope<T> scope)
        {
            children.Add(name, scope);
        }

        public Scope<T> Lookup(string name)
        {
            if (children.ContainsKey(name)) return children[name];
            if (IsRoot) return null;
            return parentScope.Lookup(name);
        }

        public Scope<T> Lookup(params string[] names)
        {
            return Lookup(names.AsEnumerable());
        }

        private Scope<T> Lookup(IEnumerable<string> names)
        {
            if (names.Any())
            {
                var first = names.First();
                var rest = names.Skip(1);
                if (children.ContainsKey(first))
                {
                    var result = children[first].Lookup(rest);
                    if (result != null) return result;
                }
                if (!IsRoot)
                {
                    var result = parentScope.Lookup(names);
                    return result;
                }
                return null;
            }
            return this;
        }
    }
}