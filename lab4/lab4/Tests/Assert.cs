using lab4.Tests.Assertions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace lab4.Tests.Assertions
{
    public static class Assert
    {
        public static void AreEqual<T>(T expected, T actual, string? message = null)
        {
            if (!Equals(expected, actual))
            {
                throw new AssertException(message ?? $"Assert.AreEqual failed. Expected: {expected}, Actual: {actual}");
            }
        }
        public static void AreNotEqual<T>(T notExpected, T actual, string? message = null)
        {
            if (Equals(notExpected, actual))
            {
                throw new AssertException(message ?? $"Assert.AreNotEqual failed. Value: {actual}");
            }
        }
        public static void IsTrue(bool condition, string? message = null)
        {
            if (!condition)
            {
                throw new AssertException(message ?? "Assert.IsTrue failed.");
            }
        }
        public static void IsFalse(bool condition, string? message = null)
        {
            if (condition)
            {
                throw new AssertException(message ?? "Assert.IsFalse failed.");
            }
        }

        public static void IsNull(object? value, string? message = null)
        {
            if (value != null)
            {
                throw new AssertException(message ?? $"Assert.IsNull failed. Value: {value}");
            }
        }
        public static void IsNotNull(object? value, string? message = null)
        {
            if (value == null)
            {
                throw new AssertException(message ?? "Assert.IsNotNull failed.");
            }
        }
        public static void That(Expression<Func<bool>> expression)
        {
            bool result = expression.Compile().Invoke();
            if (result)
                return;

            string details = ExplainExpression(expression.Body);
            throw new AssertException($"Assert.That failed. {details}");
        }
        private static string ExplainExpression(Expression expression)
        {
            if (expression is BinaryExpression binary)
            {
                object? leftValue = Evaluate(binary.Left);
                object? rightValue = Evaluate(binary.Right);

                return $"Expression: {binary.Left} {binary.NodeType} {binary.Right}; " +
                       $"Left = {FormatValue(leftValue)}, Right = {FormatValue(rightValue)}, Operator = {binary.NodeType}";
            }

            object? value = Evaluate(expression);
            return $"Expression: {expression}; Evaluated value = {FormatValue(value)}";
        }
        private static object? Evaluate(Expression expression)
        {
            try
            {
                var converted = Expression.Convert(expression, typeof(object));
                var lambda = Expression.Lambda<Func<object?>>(converted);
                return lambda.Compile().Invoke();
            }
            catch
            {
                return "<cannot evaluate>";
            }
        }
        private static string FormatValue(object? value)
        {
            return value == null ? "null" : value.ToString() ?? "null";
        }
    }
}

