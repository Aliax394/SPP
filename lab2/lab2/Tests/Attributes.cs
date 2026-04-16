using System;
using System.Collections.Generic;
using System.Text;

namespace lab2.Tests
{
        [AttributeUsage(AttributeTargets.Method)]
        public class BeforeAllAttr : Attribute { }

        [AttributeUsage(AttributeTargets.Method)]
        public class AfterAllAttr : Attribute { }
        [AttributeUsage(AttributeTargets.Class)]
        public class TestClassAttr : Attribute { }

        [AttributeUsage(AttributeTargets.Method)]
        public class TestAttr : Attribute
        {
            public int Priority { get; set; }
            public bool Enabled { get; set; } = true;
        }

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
        public class TestCaseAttr : Attribute
        {
            public object[] Parameters { get; }

            public TestCaseAttr(params object[] parameters)
            {
                Parameters = parameters;
            }
        }

        [AttributeUsage(AttributeTargets.Method)]
        public class TestInfoAttr : Attribute
        {
            public string Author { get; set; }
            public string Category { get; set; }
        }
        [AttributeUsage(AttributeTargets.Method)]
        public class TimeoutAttribute : Attribute
        {
            public int Milliseconds { get; }

            public TimeoutAttribute(int milliseconds)
            {
                Milliseconds = milliseconds;
            }
        }
}

