using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Kontur.ImageTransformer.Drawing;
using Kontur.ImageTransformer.Filters;
using NUnit.Framework;

namespace Kontur.ImageTransformer.Tests.Filters
{
    [TestFixture]
    internal class ThresholdFilterTests
    {
        [TestCase(100, 100, 25)]
        [TestCase(100, 100, 50)]
        [TestCase(100, 100, 75)]
        [TestCase(250, 250, 25)]
        [TestCase(250, 250, 50)]
        [TestCase(250, 250, 75)]
        [TestCase(500, 500, 25)]
        [TestCase(500, 500, 50)]
        [TestCase(500, 500, 75)]
        [TestCase(750, 750, 25)]
        [TestCase(750, 750, 50)]
        [TestCase(750, 750, 75)]
        [TestCase(1000, 1000, 25)]
        [TestCase(1000, 1000, 50)]
        [TestCase(1000, 1000, 75)]
        public async Task ApplyThresholdFilter(int w, int h, int threshold)
        {
            var filter = new ThresholdFilter(threshold);
            using (var img = new BitmapImage(TestData.GetTestImage(w, h)))
            {
                await filter.Apply(img, img.Bounds, CancellationToken.None);
                await img.SaveAsync(File.Create(Path.Combine(TestData.RootFolder, $"threshold({threshold})-{w}x{h}.png")), CancellationToken.None);
            }
        }
    }
}