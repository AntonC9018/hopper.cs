using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Hopper.Meta
{
    public interface IThing 
    {
        string Identity { get; }
        string Location { get; }
    }

    public class ErrorContext
    {
        public Stack<IThing> things;
        public List<string> accumulatedErrors;

        public ErrorContext()
        {
            things = new Stack<IThing>();
            accumulatedErrors = new List<string>();
        }

        public bool HasErrors => accumulatedErrors.Count > 0;

        public void PushThing(IThing thing)
        {
            things.Push(thing);
        }

        public void PopThing()
        {
            things.Pop();
        }

        public void ClearErrors()
        {
            accumulatedErrors.Clear();
        }

        public void WriteErrorPrefix()
        {
            Console.Write("An error has occured");
            foreach (var thing in things)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" at ");

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(thing.Identity);

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" at ");

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(thing.Location);
            }
        }

        public void Report(string errorText)
        {
            WriteErrorPrefix();

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(errorText);

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("Happened ");
            Console.Write(Environment.StackTrace);

        }
    }
}