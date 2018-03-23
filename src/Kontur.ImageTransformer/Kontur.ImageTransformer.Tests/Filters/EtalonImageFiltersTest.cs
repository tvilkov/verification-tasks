using System.Collections;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Kontur.ImageTransformer.Drawing;
using Kontur.ImageTransformer.Filters;
using NUnit.Framework;

namespace Kontur.ImageTransformer.Tests.Filters
{
    [TestFixture]
    internal class EtalonImageFiltersTest
    {
        [Test, TestCaseSource(typeof(EtalonImageFiltersTestData), nameof(EtalonImageFiltersTestData.TestCases))]
        public async Task ApplyFilters(ImageFilter filter, string filterName)
        {
            using (var img = new BitmapImage(TestData.GetZebraImage()))
            {
                await filter.Apply(img, img.Bounds, CancellationToken.None);
                await img.SaveAsync(File.Create(Path.Combine(TestData.RootFolder, $"zebra-{filterName}.png")), CancellationToken.None);
            }
        }
    }

    public class EtalonImageFiltersTestData
    {
        public static IEnumerable TestCases
        {
            get
            {
                yield return new TestCaseData(new GrayscaleFilter(), nameof(GrayscaleFilter)).SetName(nameof(GrayscaleFilter));
                yield return new TestCaseData(new SepiaFilter(), nameof(SepiaFilter)).SetName(nameof(SepiaFilter));
                yield return new TestCaseData(new ThresholdFilter(50), nameof(ThresholdFilter) + "-50").SetName(nameof(ThresholdFilter) + "-50");
            }
        }
    }
}