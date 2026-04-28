using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace lab4.Tests.Core
{
    public class TestExecutionEntry
    {
        public MethodInfo Method { get; set; } = null!;
        public object Instance { get; set; } = null!;
        public object?[]? Parameters { get; set; }
        public bool IsSkipped { get; set; }
        public int Priority { get; set; }
        public string Author { get; set; } = "Unknown";
        public string Category { get; set; } = "Unspecified";
        public string DisplayName { get; set; } = string.Empty;
    }
}

