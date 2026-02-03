namespace Library
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TestClassAttribute : Attribute {}
    [AttributeUsage(AttributeTargets.Method)]
    public class BeforeAttribute : Attribute {}
    public class AfterAttribute : Attribute {}
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
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestCaseAttribute : Attribute
    {
        public object?[] Parameters { get; }
        public TestCaseAttribute(params object?[] parameters) 
        { 
            Parameters = parameters;
        }
    }

}