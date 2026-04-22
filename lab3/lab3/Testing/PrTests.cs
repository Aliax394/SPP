using lab3.Project;
using lab3.Tests;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Text;

namespace lab3.Testing
{
    [TestClassAttr]
    public class PrTests
    {
        private Calculator _calculator = null!;

        [BeforeAttr]
        public void Setup()
        {
            _calculator = new Calculator();
        }

        [AfterAttr]
        public void TearDown()
        {
            // Можно оставить пустым
        }

        [TestAttr(priority: 3)]
        [TestCaseAttr(2, 3, 5)]
        [TestCaseAttr(10, 15, 25)]
        [TestCaseAttr(-1, 1, 0)]
        public void Add_Test(int a, int b, int expected)
        {
            Thread.Sleep(150); // имитация нагрузки
            AssertEx.AreEqual(expected, _calculator.Add(a, b));
        }

        [TestAttr(priority: 2)]
        [TestCaseAttr(10, 2, 5)]
        [TestCaseAttr(20, 4, 5)]
        public void Divide_Test(int a, int b, int expected)
        {
            Thread.Sleep(250);
            AssertEx.AreEqual(expected, _calculator.Divide(a, b));
        }

        [TestAttr(priority: 1)]
        public void Divide_By_Zero_Test()
        {
            Thread.Sleep(300);
            bool thrown = false;

            try
            {
                _calculator.Divide(5, 0);
            }
            catch (DivideByZeroException)
            {
                thrown = true;
            }

            AssertEx.IsTrue(thrown);
        }

        [TestAttr(priority: 2)]
        [TestCaseAttr(4, true)]
        [TestCaseAttr(7, false)]
        [TestCaseAttr(100, true)]
        public void IsEven_Test(int value, bool expected)
        {
            Thread.Sleep(120);
            AssertEx.AreEqual(expected, _calculator.IsEven(value));
        }
    }
}
