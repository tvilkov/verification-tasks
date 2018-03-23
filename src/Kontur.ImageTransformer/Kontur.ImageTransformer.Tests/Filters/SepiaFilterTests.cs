using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Kontur.ImageTransformer.Drawing;
using Kontur.ImageTransformer.Filters;
using NUnit.Framework;

namespace Kontur.ImageTransformer.Tests.Filters
{
    [TestFixture]
    internal class SepiaFilterTests
    {
        protected readonly ImageFilter Filter = new SepiaFilter();

        [TestCase(100, 100)]
        [TestCase(250, 250)]
        [TestCase(500, 500)]
        [TestCase(750, 750)]
        [TestCase(1000, 1000)]
        public async Task ApplySepiaFilter(int w, int h)
        {
            using (var img = new BitmapImage(TestData.GetTestImage(w, h)))
            {
                await Filter.Apply(img, img.Bounds, CancellationToken.None);
                await img.SaveAsync(File.Create(Path.Combine(TestData.RootFolder, $"sepia-{w}x{h}.png")), CancellationToken.None);
            }
        }
    }
}