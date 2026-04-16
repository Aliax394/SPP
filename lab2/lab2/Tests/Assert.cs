using System;
using System.Collections.Generic;
using System.Text;

namespace lab2.Tests
{
    internal class Assert
    {
            public static void IsTrue(bool condition, string? message = null)
            {
                if (!condition)
                    throw new Exception(message ?? "Assert.IsTrue failed.");
            }

            public static void IsFalse(bool condition, string? message = null)
            {
                if (condition)
                    throw new Exception(message ?? "Assert.IsFalse failed.");
            }

            public static void AreEqual<T>(T expected, T actual, string? message = null)
            {
                if (!Equals(expected, actual))
                {
                    throw new Exception(
                        message ?? $"Assert.AreEqual failed. Expected: {expected}, Actual: {actual}");
                }
            }

            public static void AreNotEqual<T>(T notExpected, T actual, string? message = null)
            {
                if (Equals(notExpected, actual))
                {
                    throw new Exception(
                        message ?? $"Assert.AreNotEqual failed. Value should not be: {actual}");
                }
            }

            public static void IsNull(object? obj, string? message = null)
            {
                if (obj != null)
                    throw new Exception(message ?? "Assert.IsNull failed. Object is not null.");
            }

            public static void IsNotNull(object? obj, string? message = null)
            {
                if (obj == null)
                    throw new Exception(message ?? "Assert.IsNotNull failed. Object is null.");
            }

            public static void Fail(string? message = null)
            {
                throw new Exception(message ?? "Assert.Fail was called.");
            }

            public static void Throws<TException>(Action action, string? message = null)
                where TException : Exception
            {
                try
                {
                    action();
                }
                catch (TException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        message ?? $"Assert.Throws failed. Expected exception: {typeof(TException).Name}, but got: {ex.GetType().Name}");
                }

                throw new Exception(
                    message ?? $"Assert.Throws failed. Expected exception: {typeof(TException).Name}, but no exception was thrown.");
            }

            public static void AreApproximatelyEqual(double expected, double actual, double delta, string? message = null)
            {
                if (Math.Abs(expected - actual) > delta)
                {
                    throw new Exception(
                        message ?? $"Assert.AreApproximatelyEqual failed. Expected: {expected}, Actual: {actual}, Delta: {delta}");
                }
            }
        }
    }

