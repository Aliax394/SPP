using lab4.TestedProject;
using lab4.Tests;
using lab4.Tests.Assertions;
using lab4.Tests.Core;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace lab4.TestsProject
{
    [TestClassAttr]
    public class CalculatorTests
    {
        private Calculator _calculator = null!;

        [Before]
        public void Setup()
        {
            _calculator = new Calculator();
        }
        [After]
        public void Cleanup()
        {
            System.Console.WriteLine("Calculator test finished");
        }

        public static IEnumerable<TestCaseData> AddCases()
        {
            yield return new TestCaseData
            {
                Arguments = new object?[] { 2, 3, 5 },
                Name = "Add_2_3_Expected_5"
            };

            yield return new TestCaseData
            {
                Arguments = new object?[] { -1, 4, 3 },
                Name = "Add_Minus1_4_Expected_3"
            };

            yield return new TestCaseData
            {
                Arguments = new object?[] { 0, 0, 0 },
                Name = "Add_0_0_Expected_0"
            };
        }
        [Test(priority: 3)]
        [Category("Math")]
        [Author("Pavel")]
        [TestCaseSource(nameof(AddCases))]
        public void Add_Works(int a, int b, int expected)
        {
            int result = _calculator.Add(a, b);
            Assert.AreEqual(expected, result);
        }

        [Test(priority: 2)]
        [Category("Math")]
        [Author("Pavel")]
        public void Divide_ByZero_Throws()
        {
            try
            {
                _calculator.Divide(10, 0);
                throw new System.Exception("Expected exception was not thrown");
            }
            catch (System.DivideByZeroException)
            {
                Assert.IsTrue(true);
            }
        }
        [Test(priority: 5)]
        [Category("Expressions")]
        [Author("Pavel")]
        public void AssertThat_ShowsDetailedExpression()
        {
            int x = 5;
            int y = 10;
            Assert.That(() => x > y);
        }
    }

}