using System;
using System.Collections.Generic;
using System.Text;

namespace lab3.Tests
{
    public class AssertFailEx : Exception
    {
        public AssertFailEx(string message) : base(message) { }
    }

    public static class AssertEx
    {
        public static void AreEqual<T>(T expected, T actual, string? message = null)
        {
            if (!Equals(expected, actual))
                throw new AssertFailEx(message ?? $"Expected: {expected}, Actual: {actual}");
        }

        public static void IsTrue(bool condition, string? message = null)
        {
            if (!condition)
                throw new AssertFailEx(message ?? "Condition is false");
        }

        public static void IsFalse(bool condition, string? message = null)
        {
            if (condition)
                throw new AssertFailEx(message ?? "Condition is true");
        }

        public static void IsNotNull(object? obj, string? message = null)
        {
            if (obj == null)
                throw new AssertFailEx(message ?? "Object is null");
        }
    }
}
