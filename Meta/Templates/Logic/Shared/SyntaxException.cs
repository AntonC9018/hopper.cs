using System;

namespace Hopper.Meta
{
    public class SyntaxException : Exception 
    {
        public SyntaxException(string message) : base(message)
        {
        }
    }
}