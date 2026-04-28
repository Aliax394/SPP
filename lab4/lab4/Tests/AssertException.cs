using System;
using System.Collections.Generic;
using System.Text;

namespace lab4.Tests.Assertions
{
    public class AssertException : Exception
    {
        public AssertException(string message) : base(message)
        {
        }
    }
}
