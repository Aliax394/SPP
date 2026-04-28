using System;
using System.Collections.Generic;
using System.Text;

namespace lab4.Tests.Core
{
    public class TestResult
    {
        public string TestName { get; set; } = string.Empty;
        public bool Passed { get; set; }
        public bool Skipped { get; set; }
        public string Message { get; set; } = string.Empty;
        public long DurationMs { get; set; }
    }
}
