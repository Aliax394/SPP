using lab2.Project;
using lab2.Tests;
using System;
using System.Collections.Generic;
using System.Text;

namespace lab2.Testing
{
    internal class PrTesting
    {
        [TestClassAttr]
        public class TemperatureConverterTests
        {
            private static TempConver _converter = null!;

            [BeforeAllAttr]
            public static void Setup()
            {
                _converter = new TempConver();
                Console.WriteLine("BeforeAll: TemperatureConverter created");
            }

            [AfterAllAttr]
            public static void Cleanup()
            {
                Console.WriteLine("AfterAll: TemperatureConverterTests finished");
            }

            [TestAttr(Priority = 1, Enabled = true)]
            [TestInfoAttr(Author = "Student", Category = "Conversion")]
            [TestCaseAttr(0.0, 32.0)]
            [TestCaseAttr(100.0, 212.0)]
            [TestCaseAttr(-40.0, -40.0)]
            public void CelsiusToFahrenheit_ShouldReturnCorrectValue(double input, double expected)
            {
                double result = _converter.CelsiusToFahrenheit(input);
                Assert.AreApproximatelyEqual(expected, result, 0.001);
            }

            [TestAttr(Priority = 2, Enabled = true)]
            [TestInfoAttr(Author = "Student", Category = "Exception")]
            public void CelsiusToKelvin_ShouldThrow_ForValueBelowAbsoluteZero()
            {
                Assert.Throws<ArgumentException>(() => _converter.CelsiusToKelvin(-300.0));
            }

            [TestAttr(Priority = 3, Enabled = true)]
            [TestInfoAttr(Author = "Student", Category = "WaterState")]
            [TestCaseAttr(-10.0, "Solid")]
            [TestCaseAttr(0.0, "Freezing point")]
            [TestCaseAttr(25.0, "Liquid")]
            [TestCaseAttr(100.0, "Boiling point")]
            public void GetWaterStateByCelsius_ShouldReturnCorrectState(double input, string expected)
            {
                string result = _converter.GetWaterStateByCelsius(input);
                Assert.AreEqual(expected, result);
            }

            [TestAttr(Priority = 4, Enabled = true)]
            [TestInfoAttr(Author = "Student", Category = "Slow")]
            [Timeout(1500)]
            public void SlowTest1_ShouldPass()
            {
                Thread.Sleep(1000);
                Assert.IsTrue(true);
            }

            [TestAttr(Priority = 5, Enabled = true)]
            [TestInfoAttr(Author = "Student", Category = "Slow")]
            [Timeout(1500)]
            public void SlowTest2_ShouldPass()
            {
                Thread.Sleep(1000);
                Assert.IsTrue(true);
            }

            [TestAttr(Priority = 6, Enabled = true)]
            [TestInfoAttr(Author = "Student", Category = "Slow")]
            [Timeout(1500)]
            public void SlowTest3_ShouldPass()
            {
                Thread.Sleep(1000);
                Assert.IsTrue(true);
            }

            [TestAttr(Priority = 7, Enabled = true)]
            [TestInfoAttr(Author = "Student", Category = "Slow")]
            [Timeout(1500)]
            public void SlowTest4_ShouldPass()
            {
                Thread.Sleep(1000);
                Assert.IsTrue(true);
            }

            [TestAttr(Priority = 8, Enabled = true)]
            [TestInfoAttr(Author = "Student", Category = "Timeout")]
            [Timeout(500)]
            public void SlowTest5_ShouldTimeout()
            {
                Thread.Sleep(1200);
                Assert.IsTrue(true);
            }

            [TestAttr(Priority = 9, Enabled = false)]
            [TestInfoAttr(Author = "Student", Category = "Disabled")]
            public void Disabled_Test_Example()
            {
                Assert.Fail("This test should not run.");
            }
        }
    }
}
