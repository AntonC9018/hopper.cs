using System.Collections.Generic;

namespace Hopper.Meta.Stats
{
    public class ParsingContext
    {
        public string fileName;
        public List<string> tokenNames;
        public Scope<StatType> scope;

        public ParsingContext(string fileName)
        {
            this.tokenNames = new List<string>();
            this.fileName = fileName;
            this.scope = new Scope<StatType>(null);
        }

        public void PushScope(string scopeName)
        {
            var newScope = new Scope<StatType>(null, scope);
            scope.Add(scopeName, newScope);
            scope = newScope;
        }
        public void PushScope(StatType stat)
        {
            if (scope.children.ContainsKey(stat.name))
            {
                var newScope = scope.children[stat.name];
                newScope.value = stat;
                scope = newScope;
            }
            else
            {
                var newScope = new Scope<StatType>(stat, scope);
                scope.Add(stat.name, newScope);
                scope = newScope;
            }
        }

        public void PopScope() => scope = scope.parentScope;

        public void Push(string tokenName) => tokenNames.Add(tokenName);
        public void Pop() => tokenNames.RemoveAt(tokenNames.Count - 1);

        public void Report(string error)
        {
            throw new SyntaxException($"Error while parsing {fileName} at {System.String.Join(".", tokenNames)}: {error}");
        }
    }
}