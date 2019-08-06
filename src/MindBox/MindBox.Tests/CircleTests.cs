using System;
using System.Collections;
using NUnit.Framework;

namespace MindBox.Tests
{
    [TestFixture]
    public class CircleTests
    {
        [Test]
        public void CanCreateCircle()
        {
            var circle = new Circle(123.456);
            Assert.IsNotNull(circle);
            Assert.AreEqual(123.456, circle.Radius, "Created circle has wrong radius");
        }

        [TestCase(0, TestName = "Radius=0")]
        [TestCase(0, TestName = "Radius=-1")]
        [TestCase(0, TestName = "Radius=-123.456789")]
        public void CircleWithIncorrectRadiusCanNotBeCreated(double radius)
        {
            // ReSharper disable once ObjectCreationAsStatement
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Circle(radius));

            Assert.IsNotNull(ex, "Expected exception was not thrown");
            Assert.AreEqual(nameof(radius), ex.ParamName, "Unexpected bad parameter name");
            StringAssert.Contains("expected to be a positive number", ex.Message, "Unexpected error message");
        }

        [TestCaseSource(nameof(CalculateAreaTestCases))]
        public void CalculateAreaTest(double radius, double expectedArea)
        {
            var area = new Circle(radius).CalculateArea();

            Assert.AreEqual(expectedArea, area, Algorithms.Epsilon, "Area calculation failed");
        }

        public static IEnumerable CalculateAreaTestCases
        {
            get
            {
                yield return new TestCaseData(1, Math.PI * 1 * 1);
                yield return new TestCaseData(9.99, Math.PI * 9.99 * 9.99);
                yield return new TestCaseData(123.456789, Math.PI * 123.456789 * 123.456789);
                yield return new TestCaseData(999999.999999, Math.PI * 999999.999999 * 999999.999999);
            }
        }

        [Test]
        public void ToStringTest()
        {
            var str = new Circle(123.456).ToString();
            StringAssert.AreEqualIgnoringCase(str, "Circle (Radius=123.456)");
        }
    }
}