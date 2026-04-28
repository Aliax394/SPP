using System;
using System.Collections.Generic;
using System.Text;

namespace lab4.Tests
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TestClassAttr : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class TestAttribute : Attribute
    {
        public bool Enabled { get; }
        public int Priority { get; }

        public TestAttribute(bool enabled = true, int priority = 0)
        {
            Enabled = enabled;
            Priority = priority;
        }
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class CategoryAttribute : Attribute
    {
        public string Name { get; }

        public CategoryAttribute(string name)
        {
            Name = name;
        }
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorAttribute : Attribute
    {
        public string Name { get; }

        public AuthorAttribute(string name)
        {
            Name = name;
        }
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class TestCaseSourceAttribute : Attribute
    {
        public string SourceName { get; }

        public TestCaseSourceAttribute(string sourceName)
        {
            SourceName = sourceName;
        }

    }
    [AttributeUsage(AttributeTargets.Method)]
    public class BeforeAttribute : Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Method)]
    public class AfterAttribute : Attribute
    {
    }
}