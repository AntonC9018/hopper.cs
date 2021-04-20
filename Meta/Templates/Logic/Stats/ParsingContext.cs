using System.Collections.Generic;

namespace Hopper.Meta.Stats
{
    public class ParsingContext
    {
        public string fileName;
        public List<string> tokenNames;
        public Scope<StatType> scope;

        public ParsingContext(string rootScopeName)
        {
            this.tokenNames = new List<string>();
            this.scope = new Scope<StatType>(rootScopeName, null, null);
        }

        public void ResetFileName(string fileName)
        {
            this.fileName = fileName;
        }

        public void PushScope(string scopeName)
        {
            scope = scope.Add(scopeName, null);
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
                scope = scope.Add(stat.name, stat);
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