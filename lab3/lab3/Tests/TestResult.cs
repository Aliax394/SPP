using System;
using System.Collections.Generic;
using System.Text;

namespace lab3.Tests
{
    public class TestResult
    {
        public string className { get; set; } = "";
        public string methodName { get; set; } = "";
        public bool passed { get; set; }
        public string message { get; set; } = "";
        public long durationMs { get; set; }
        public int workerId { get; set; }
    }
}
