using System.Collections.Generic;
using System.Linq;

namespace Hopper.Meta.Stats
{
    public class Scope<T>
    {
        public Dictionary<string, Scope<T>> children; 
        public Scope<T> parentScope;
        public string name;
        public T value; // may have a stat defined here

        public Scope(string name, T value, Scope<T> parentScope)
        {
            this.name = name;
            this.value = value;
            this.parentScope = parentScope;
            this.children = new Dictionary<string, Scope<T>>();
        }

        public bool IsRoot => parentScope == null;

        public Scope<T> Add(string name, T value)
        {
            var scope = new Scope<T>(name, value, this);
            AddScope(name, scope);
            return scope;
        }

        public void AddScope(string name, Scope<T> scope)
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
                
                if (first == name && IsRoot)
                {
                    return Lookup(rest);
                }
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

        public IEnumerable<T> GetAllChildrenValues()
        {
            if (value != null) yield return value;
            
            foreach (var kvp in children)
            foreach (var value in kvp.Value.GetAllChildrenValues())
            {
                yield return value;
            }
        }
    }
}