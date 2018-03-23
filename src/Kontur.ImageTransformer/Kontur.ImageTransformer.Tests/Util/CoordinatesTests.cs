using System.Collections.Generic;
using System.Drawing;
using Kontur.ImageTransformer.Util;
using NUnit.Framework;

namespace Kontur.ImageTransformer.Tests.Util
{
    [TestFixture]
    internal class CoordinatesTests
    {
        [TestCase("0,0,0,0", 0, 0, 0, 0)]
        [TestCase("-0,-0,-0,-0", 0, 0, 0, 0)]
        [TestCase("01,02,03,04", 1, 2, 3, 4)]
        [TestCase("-01,-02,-03,-04", -1, -2, -3, -4)]
        [TestCase("99999,99998,99997,99996", 99999, 99998, 99997, 99996)]
        [TestCase("-99999,-99998,-99997,-99996", -99999, -99998, -99997, -99996)]
        [TestCase(" 1, 2,\t-3,\n-4 ", 1, 2, -3, -4)]
        public void TryParsePositiveTest(string input, int x, int y, int w, int h)
        {
            var parsed = Coordinates.TryParse(input, out var result);
            Assert.IsTrue(parsed);
            Assert.AreEqual(x, result.X);
            Assert.AreEqual(y, result.Y);
            Assert.AreEqual(w, result.W);
            Assert.AreEqual(h, result.H);
        }

        [TestCase("")]
        [TestCase("bad-format#1")]
        [TestCase("1;2;3;4")]
        [TestCase("1")]
        [TestCase("1,2")]
        [TestCase("1,2,3")]
        [TestCase("1,2,3,4,5")]
        [TestCase("1.10,2,3,4")]
        [TestCase("1,2,3,ff")]
        [TestCase("-,2,3,4")]
        [TestCase("1,,3,4")]
        public void TryParseNegativeTest(string input)
        {
            var parsed = Coordinates.TryParse(input, out var _);
            Assert.IsFalse(parsed);
        }

        [Test, TestCaseSource(nameof(ToRectangleTestCases))]
        public void ToRectangleTest(Coordinates coords, Rectangle expectedRectangle)
        {
            var rect = coords.ToRectangle();
            Assert.AreEqual(expectedRectangle, rect);
        }

        public static IEnumerable<TestCaseData> ToRectangleTestCases
        {
            get
            {
                yield return new TestCaseData(new Coordinates(0, 0, 0, 0), Rectangle.Empty);
                yield return new TestCaseData(new Coordinates(0, 0, 100, 50), new Rectangle(0, 0, 100, 50));
                yield return new TestCaseData(new Coordinates(100, 50, 100, 50), new Rectangle(100, 50, 100, 50));
                yield return new TestCaseData(new Coordinates(-100, -50, 100, 50), new Rectangle(-100, -50, 100, 50));
                yield return new TestCaseData(new Coordinates(0, 0, -100, 50), new Rectangle(-100, 0, 100, 50));
                yield return new TestCaseData(new Coordinates(0, 0, 100, -50), new Rectangle(0, -50, 100, 50));
                yield return new TestCaseData(new Coordinates(0, 0, -100, -50), new Rectangle(-100, -50, 100, 50));
                yield return new TestCaseData(new Coordinates(-20, -10, -100, -50), new Rectangle(-120, -60, 100, 50));
            }
        }
    }
}