using System;
using System.Collections.Generic;

namespace Hopper.Meta.Stats
{
    public class ParsingContext
    {
        public string fileName;
        public List<string> tokenNames;
        public Scope<StatType> currentScope;
        public Scope<StatType> rootScope;

        public ParsingContext(string rootScopeName)
        {
            this.tokenNames = new List<string>();
            this.rootScope = new Scope<StatType>(rootScopeName, null, null);
            this.currentScope = rootScope;
        }

        public void ResetToRootScope()
        {
            currentScope = rootScope;
        }

        public void ResetFileName(string fileName)
        {
            this.fileName = fileName;
        }

        public void PushScope(string scopeName)
        {
            if (currentScope.children.TryGetValue(scopeName, out var newScope))
            {
                currentScope = newScope;
            }
            else
            {
                currentScope = currentScope.Add(scopeName, null);
            }
        }

        public void PushScope(StatType stat)
        {
            if (currentScope.children.ContainsKey(stat.name))
            {
                var newScope = currentScope.children[stat.name];
                newScope.value = stat;
                currentScope = newScope;
            }
            else
            {
                currentScope = currentScope.Add(stat.name, stat);
            }
        }

        public void PopScope() => currentScope = currentScope.parentScope;

        public void Push(string tokenName) => tokenNames.Add(tokenName);
        public void Pop() => tokenNames.RemoveAt(tokenNames.Count - 1);


        // TODO: this has already been implemented in the error context, so try and use that one instead.
        public bool Flag;
        public void ClearFlag() => Flag = false;

        public void Report(string error)
        {
            Console.WriteLine($"Error while parsing {fileName} at {System.String.Join(".", tokenNames)}: {error}");
            Flag = true;
        }
    }
}