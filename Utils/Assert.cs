using System;
using System.Collections.Generic;

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
        public static void That(bool expression, string message = "")
        {
            if (expression == false)
            {
                throw new Exception(message);
            }
        }

        public static void AreEqual<T>(T expected, T actual, string message = "")
            where T : IEquatable<T>
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
            {
                throw new Exception(message + $"\nExpected {expected}, got {actual}.");
            }
        }

        public static void AreNotEqual<T>(T expected, T actual, string message = "")
            where T : IEquatable<T>
        {
            if (EqualityComparer<T>.Default.Equals(expected, actual))
            {
                throw new Exception(message + $"\nExpected {expected}, got {actual}.");
            }
        }
    }
}