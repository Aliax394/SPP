using System;
using System.Collections.Generic;
using System.Text;

namespace lab4.TestedProject
{
    public class Calculator
    {
        public int Add(int a, int b) => a + b;
        public int Subtract(int a, int b) => a - b;
        public int Multiply(int a, int b) => a * b;

        public int Divide(int a, int b)
        {
            if (b == 0)
                throw new DivideByZeroException("Division by zero");

            return a / b;
        }
    }
}
