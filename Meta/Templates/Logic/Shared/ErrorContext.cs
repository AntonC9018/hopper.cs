using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Hopper.Meta
{
    public interface IThing 
    {
        string Identity { get; }
        string Location { get; }
    }

    public class SymbolThing : IThing
    {
        public ISymbol symbol;

        public SymbolThing(ISymbol symbol)
        {
            this.symbol = symbol;
        }

        public string Identity => $"{symbol.Name} {symbol.Kind}";
        public string Location => $"{symbol.Locations.First()}";
    }

    public class ErrorContext
    {
        private Stack<IThing> things;
        public bool Flag;

        public ErrorContext()
        {
            things = new Stack<IThing>();
        }

        public void PushThing(IThing thing)
        {
            things.Push(thing);
        }

        public void PushThing(ISymbol symbol)
        {
            things.Push(new SymbolThing(symbol));
        }

        public void PopThing()
        {
            things.Pop();
        }

        private void WriteErrorLocation()
        {
            foreach (var thing in things)
            {
                Console.WriteLine($"  at {thing.Identity} at {thing.Location}");
            }
        }

        public void ClearFlag() => Flag = false;

        public void Report(string errorText)
        {
            Flag = true;
            Console.WriteLine(errorText);
            WriteErrorLocation();
        }
    }
}