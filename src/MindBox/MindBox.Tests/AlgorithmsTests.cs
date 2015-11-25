using System;
using NUnit.Framework;

namespace MindBox.Tests
{
    public class AlgorithmsTests
    {
        private const double CUSTOM_EPSILON = 1e-10;   // 0.000..1,  10 -digits after decimal point

        [TestFixture]
        public class CalculateTriangleAreaTests
        {
            [Test]
            [TestCase(-1, 1, 1, "sideA: length expected to be a positive value", "sideA")]
            [TestCase(1, -1, 1, "sideB: length expected to be a positive value", "sideB")]
            [TestCase(1, 1, -1, "sideC: length expected to be a positive value", "sideC")]
            public void ShouldFailIfSideLengthIsNegative(double sideA, double sideB, double sideC, string expectedErrorMessage, string expectedInvalidParamName)
            {
                var ex = Assert.Throws<ArgumentException>(() => Algorithms.CalculateTriangleArea(sideA, sideB, sideC));

                Assert.IsNotNull(ex);
                Assert.AreEqual(expectedInvalidParamName, ex.ParamName);
                StringAssert.Contains(expectedErrorMessage, ex.Message);
            }

            [Test]
            [TestCase(0, 1, 1, "sideA: length expected to be a positive value", "sideA")]
            [TestCase(1, 0, 1, "sideB: length expected to be a positive value", "sideB")]
            [TestCase(1, 1, 0, "sideC: length expected to be a positive value", "sideC")]
            public void ShouldFailIfSideLengthIsZero(double sideA, double sideB, double sideC, string expectedErrorMessage, string expectedInvalidParamName)
            {
                var ex = Assert.Throws<ArgumentException>(() => Algorithms.CalculateTriangleArea(sideA, sideB, sideC));

                Assert.IsNotNull(ex);
                Assert.AreEqual(expectedInvalidParamName, ex.ParamName);
                StringAssert.Contains(expectedErrorMessage, ex.Message);
            }

            [Test]
            [TestCase(1, 1, 5)]
            [TestCase(1, 3, 5)]
            [TestCase(1, 4, 5)]
            public void ShouldFailIfGivenTriangleNotExist(double sideA, double sideB, double sideC)
            {
                var ex = Assert.Throws<InvalidOperationException>(() => Algorithms.CalculateTriangleArea(sideA, sideB, sideC));

                Assert.IsNotNull(ex);
                Assert.AreEqual("Triangle doesn't exist", ex.Message);
            }

            [Test]
            [TestCase(double.Epsilon, double.Epsilon, double.Epsilon, 0, TestName = "CanHandleBorderlineCasesWithoutError: double.Epsilon")]
            [TestCase(double.MaxValue, double.MaxValue, double.MaxValue, double.PositiveInfinity, TestName = "CanHandleBorderlineCasesWithoutError: double.MaxValue")]
            public void CanHandleBorderlineCasesWithoutError(double sideA, double sideB, double sideC, double expectedArea)
            {
                var area = Algorithms.CalculateTriangleArea(sideA, sideB, sideC);
                Assert.AreEqual(area, expectedArea);
            }

            [TestCase(1, 1, 1.414213562373095, 0.5)]
            [TestCase(10, 20, 25, 94.99177595981665)]
            [TestCase(100, 200, 250, 9499.177595981664)]
            [TestCase(99.99, 99.99, 99.99, 4329.26103681968)]
            [TestCase(0.99, 0.99, 0.99, 0.424395749124564)]
            public void CalculateAreaForValidTriangle(double sideA, double sideB, double sideC, double expectedArea)
            {
                var area = Algorithms.CalculateTriangleArea(sideA, sideB, sideC);
                var diff = Math.Abs(area - expectedArea);
                //Debug.WriteLine("Actual: " + area + ", expected: " + expectedArea);
                Assert.IsTrue(diff <= CUSTOM_EPSILON);
            }
        }

        [TestFixture]
        public class TryCalculateTriangleAreaTests
        {
            [Test]
            [TestCase(-1, 1, 1)]
            [TestCase(1, -1, 1)]
            [TestCase(1, 1, -1)]
            public void ShouldReturnFalseIfSideLengthIsNegative(double sideA, double sideB, double sideC)
            {
                double area;
                var result = Algorithms.TryCalculateTriangleArea(sideA, sideB, sideC, out area);

                Assert.IsFalse(result);
                Assert.AreEqual(0, area);
            }

            [Test]
            [TestCase(0, 1, 1)]
            [TestCase(1, 0, 1)]
            [TestCase(1, 1, 0)]
            public void ShouldReturnFalseSideLengthIsZero(double sideA, double sideB, double sideC)
            {
                double area;
                var result = Algorithms.TryCalculateTriangleArea(sideA, sideB, sideC, out area);

                Assert.IsFalse(result);
                Assert.AreEqual(0, area);
            }

            [Test]
            [TestCase(1, 1, 5)]
            [TestCase(1, 3, 5)]
            [TestCase(1, 4, 5)]
            public void ShouldReturnFalseIfGivenTriangleNotExist(double sideA, double sideB, double sideC)
            {
                double area;
                var result = Algorithms.TryCalculateTriangleArea(sideA, sideB, sideC, out area);

                Assert.IsFalse(result);
                Assert.AreEqual(0, area);
            }

            [Test]
            [TestCase(double.Epsilon, double.Epsilon, double.Epsilon, 0, TestName = "CanHandleBorderlineCasesWithoutError: double.Epsilon")]
            [TestCase(double.MaxValue, double.MaxValue, double.MaxValue, double.PositiveInfinity, TestName = "CanHandleBorderlineCasesWithoutError: double.MaxValue")]
            public void CanHandleBorderlineCasesWithoutError(double sideA, double sideB, double sideC, double expectedArea)
            {
                double area;
                var result = Algorithms.TryCalculateTriangleArea(sideA, sideB, sideC, out area);

                Assert.IsTrue(result);
                Assert.AreEqual(area, expectedArea);
            }

            [TestCase(1, 1, 1.414213562373095, 0.5)]
            [TestCase(10, 20, 25, 94.99177595981665)]
            [TestCase(100, 200, 250, 9499.177595981664)]
            [TestCase(99.99, 99.99, 99.99, 4329.26103681968)]
            [TestCase(0.99, 0.99, 0.99, 0.424395749124564)]
            public void CalculateAreaForValidTriangle(double sideA, double sideB, double sideC, double expectedArea)
            {
                double area;
                var result = Algorithms.TryCalculateTriangleArea(sideA, sideB, sideC, out area);
                Assert.IsTrue(result);
                var diff = Math.Abs(area - expectedArea);
                Assert.IsTrue(diff <= CUSTOM_EPSILON);
            }
        }
    }
}