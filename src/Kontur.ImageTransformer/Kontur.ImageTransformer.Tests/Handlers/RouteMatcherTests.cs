using System;
using System.Collections;
using Kontur.ImageTransformer.Drawing;
using Kontur.ImageTransformer.Handlers;
using Kontur.ImageTransformer.Util;
using NUnit.Framework;

namespace Kontur.ImageTransformer.Tests.Handlers
{
    [TestFixture]
    internal class RouteMatcherTests
    {

        [Test, TestCaseSource(nameof(CanMatchImageTransformRequestTestCases))]
        public void CanMatchImageTransformRequest(string httpMethod, Uri requestUri, TransformType expectedTransformType, Coordinates expectedCoordinates)
        {
            var matched = RouteMatcher.TryMatchImageTransformRequest(httpMethod, requestUri, out var transformType, out var coords);
            Assert.IsTrue(matched, "Request didn't match");
            Assert.AreEqual(expectedTransformType, transformType);
            Assert.AreEqual(expectedTransformType, transformType);
        }

        public static IEnumerable CanMatchImageTransformRequestTestCases
        {
            get
            {
                yield return new TestCaseData("POST", new Uri("http://localhost/process/rotate-cw/0,0,0,0"), TransformType.RotateCw, Coordinates.Empty).SetName("rotate-cw");
                yield return new TestCaseData("POST", new Uri("http://localhost/process/rotate-ccw/0,0,0,0", UriKind.RelativeOrAbsolute), TransformType.RotateCcw,
                    Coordinates.Empty).SetName("rotate-ccw");
                yield return new TestCaseData("POST", new Uri("http://localhost/process/flip-h/0,0,0,0", UriKind.RelativeOrAbsolute), TransformType.FlipHorizontal,
                    Coordinates.Empty).SetName("flip-h");
                yield return new TestCaseData("POST", new Uri("http://localhost/process/flip-v/0,0,0,0", UriKind.RelativeOrAbsolute), TransformType.FlipVertical, Coordinates.Empty)
                    .SetName("flip-v");

                yield return new TestCaseData("post", new Uri("http://localhost/PROCESS/ROTATE-CW/0,0,0,0"), TransformType.RotateCw, Coordinates.Empty).SetName(
                    "rotate-cw (case insensetive)");
                yield return new TestCaseData("post", new Uri("http://localhost/PROCESS/ROTATE-CCW/0,0,0,0", UriKind.RelativeOrAbsolute), TransformType.RotateCcw,
                    Coordinates.Empty).SetName("rotate-ccw (case insensetive)");
                yield return new TestCaseData("post", new Uri("http://localhost/PROCESS/FLIP-H/0,0,0,0", UriKind.RelativeOrAbsolute), TransformType.FlipHorizontal,
                    Coordinates.Empty).SetName("flip-h (case insensetive)");
                yield return new TestCaseData("post", new Uri("http://localhost/PROCESS/FLIP-V/0,0,0,0", UriKind.RelativeOrAbsolute), TransformType.FlipVertical, Coordinates.Empty)
                    .SetName("flip-v (case insensetive)");
            }
        }
    }
}