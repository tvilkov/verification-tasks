using System;
using NUnit.Framework;

namespace MindBox.Tests
{
    [TestFixture]
    public class TriangleTests
    {
        [Test]
        public void CanCreateTriangle()
        {
            var triangle = new Triangle(5.45, 6.45, 9.99);
            Assert.IsNotNull(triangle);
            Assert.AreEqual(5.45, triangle.A, "Created triangle has wrong size of side A");
            Assert.AreEqual(6.45, triangle.B, "Created triangle has wrong size of side A");
            Assert.AreEqual(9.99, triangle.C, "Created triangle has wrong size of side A");
        }

        [TestCase(0, 1, 1, "a", TestName = "Side A is zero")]
        [TestCase(-1, 1, 1, "a", TestName = "Side A is negative")]
        [TestCase(1, 0, 1, "b", TestName = "Side B is zero")]
        [TestCase(1, -1, 1, "b", TestName = "Side B is negative")]
        [TestCase(1, 1, 0, "c", TestName = "Side C is zero")]
        [TestCase(1, 1, -1, "c", TestName = "Side C is negative")]
        public void TriangleWithIncorrectSidesCanNotBeCreated(double a, double b, double c, string expectedParamName)
        {
            // ReSharper disable once ObjectCreationAsStatement
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Triangle(a, b, c));

            Assert.IsNotNull(ex, "Expected expection was not thrown");
            StringAssert.Contains("Expected to be a positive number", ex.Message);
            Assert.AreEqual(expectedParamName, ex.ParamName, "Incorrect parameter name is not what was expected");
        }

        [Test]
        public void TriangleWhichDoesNotExistCanNotBeConstructed()
        {
            // ReSharper disable once ObjectCreationAsStatement
            var ex = Assert.Throws<InvalidOperationException>(() => new Triangle(1, 1, 4));

            Assert.IsNotNull(ex, "Expected expection was not thrown");
            StringAssert.Contains("A triangle with given sides doesn't exist", ex.Message);
        }

        [Test]
        public void IsRightTriangleTest()
        {
            var normal = new Triangle(1.5, 1.5, 2.5);
            var right = new Triangle(1.5, 1.5, Math.Sqrt(1.5 * 1.5 + 1.5 * 1.5));

            Assert.IsFalse(normal.IsRightTriangle);
            Assert.IsTrue(right.IsRightTriangle);
        }

        [TestCase(1, 1, 1.414213562373095, 0.5)]
        [TestCase(10, 20, 25, 94.99177595981665)]
        [TestCase(100, 200, 250, 9499.177595981664)]
        [TestCase(99.99, 99.99, 99.99, 4329.26103681968)]
        [TestCase(0.99, 0.99, 0.99, 0.424395749124564)]
        public void CalculateAreaTests(double a, double b, double c, double expectedArea)
        {
            var area = new Triangle(a, b, c).CalculateArea();
            Assert.AreEqual(expectedArea, area, Algorithms.Epsilon, "Area calculation failed");
        }

        [Test]
        public void ToStringTest()
        {
            var str = new Triangle(5.45, 6.45, 9.99).ToString();
            StringAssert.AreEqualIgnoringCase(str, "Triangle (A=5.45 B=6.45 C=9.99");
        }
    }
}