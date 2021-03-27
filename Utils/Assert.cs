using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Hopper.Utils
{
    public class Exception : System.Exception
    {
        public Exception(string message) : base(message)
        {
        }
    }

    public static class Assert
    {
        [Conditional("DEBUG")]
        public static void That(bool expression, string message = "")
        {
            if (expression == false)
            {
                throw new GeneratorException(message);
            }
        }

        [Conditional("DEBUG")]
        public static void AreEqual<T>(T expected, T actual, string message = "")
            where T : IEquatable<T>
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
            {
                throw new GeneratorException($"{message}\nExpected {expected}, got {actual}.");
            }
        }

        [Conditional("DEBUG")]
        public static void AreNotEqual<T>(T expected, T actual, string message = "")
            where T : IEquatable<T>
        {
            if (EqualityComparer<T>.Default.Equals(expected, actual))
            {
                throw new GeneratorException($"{message}\nExpected {expected}, got {actual}.");
            }
        }
    }
}