using System;
using System.Collections.Generic;
using System.Text;

namespace lab3.Tests
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TestClassAttr : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class TestAttr : Attribute
    {
        public bool Enabled { get; }
        public int Priority { get; }

        public TestAttr(bool enabled = true, int priority = 0)
        {
            Enabled = enabled;
            Priority = priority;
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestCaseAttr : Attribute
    {
        public object?[] Parameters { get; }

        public TestCaseAttr(params object?[] parameters)
        {
            Parameters = parameters;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class BeforeAttr : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class AfterAttr : Attribute
    {
    }
}
