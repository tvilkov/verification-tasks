using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Kontur.ImageTransformer.Drawing;
using NUnit.Framework;

namespace Kontur.ImageTransformer.Tests.Drawing
{
    [TestFixture]
    internal class BitmapImageTests
    {
        [TestCase(100, 100)]
        [TestCase(250, 250)]
        [TestCase(500, 500)]
        [TestCase(750, 750)]
        [TestCase(1000, 1000)]
        public void CanCreateImageFromBitmap(int w, int h)
        {
            using (new BitmapImage(TestData.GetTestImage(w, h)))
            {
            }
        }

        [TestCase(100, 100)]
        [TestCase(250, 250)]
        [TestCase(500, 500)]
        [TestCase(750, 750)]
        [TestCase(1000, 1000)]
        public void ImageCreatedFromBitmapHasValidSize(int w, int h)
        {
            using (var img = new BitmapImage(TestData.GetTestImage(w, h)))
            {
                Assert.AreEqual(w, img.Width);
                Assert.AreEqual(h, img.Height);
                Assert.AreEqual(new Rectangle(0, 0, w, h), img.Bounds);
            }
        }

        [TestCase(100, 100)]
        [TestCase(1000, 1000)]
        public void GetPixelReturnsCorrectColor(int w, int h)
        {
            using (var etalon = TestData.GetTestImage(w, h))
            using (var img = new BitmapImage(TestData.GetTestImage(w, h)))
            {
                for (var y = 0; y < h; y++)
                for (var x = 0; x < h; x++)
                {
                    var etalonColor = etalon.GetPixel(x, y);
                    var imgColor = img.GetPixel(x, y);
                    Assert.AreEqual(etalonColor, imgColor, $"Pixel color mismatch at ({x},{y})");
                }
            }
        }

        [TestCase(100, 100)]
        [TestCase(1000, 1000)]
        public async Task SetPixelChangesColorOfUnderliingBitmap(int w, int h)
        {
            using (var ms = new MemoryStream(w * h * 32))
            {
                var newColor = Color.FromArgb(155, 100, 100, 100);
                using (var img = new BitmapImage(TestData.GetTestImage(w, h)))
                {
                    for (var y = 0; y < h; y++)
                    for (var x = 0; x < h; x++)
                    {
                        img.SetPixel(x, y, newColor);
                    }

                    await img.SaveAsync(ms, CancellationToken.None);
                }

                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);

                using (var nativeImage = Image.FromStream(ms, true))
                using (var bitmap = new Bitmap(nativeImage))
                {
                    for (var y = 0; y < h; y++)
                    for (var x = 0; x < h; x++)
                    {
                        var color = bitmap.GetPixel(x, y);
                        Assert.AreEqual(newColor, color, $"Pixel color mismatch at ({x},{y})");
                    }
                }
            }
        }

        [TestCase(100, 100)]
        [TestCase(1000, 1000)]
        public void GetPixelAfterSetPixelReturnsUpdatedColor(int w, int h)
        {
            using (var img = new BitmapImage(TestData.GetTestImage(w, h)))
            {
                var newColor = Color.FromArgb(155, 100, 100, 100);
                for (var y = 0; y < h; y++)
                for (var x = 0; x < h; x++)
                {
                    var oldColor = img.GetPixel(x, y);
                    Assert.AreNotEqual(oldColor, newColor);
                    img.SetPixel(x, y, newColor);
                    var updatedColor = img.GetPixel(x, y);
                    Assert.AreEqual(newColor, updatedColor, $"Updated color can't be read at ({x},{y})");
                }
            }
        }

        [TestCase(TransformType.RotateCw)]
        [TestCase(TransformType.RotateCcw)]
        [TestCase(TransformType.FlipHorizontal)]
        [TestCase(TransformType.FlipVertical)]
        public void ApplyTransformTests(TransformType transformType)
        {
            using (var img = new BitmapImage(TestData.GetZebraImage()))
            using (var etalon = TestData.GetZebraImage(transformType))
            {
                img.Transform(transformType);
                Assert.AreEqual(etalon.Width, img.Width);
                Assert.AreEqual(etalon.Height, img.Height);
                for (var y = 0; y < etalon.Height; y++)
                for (var x = 0; x < etalon.Width; x++)
                {
                    var etalonColor = etalon.GetPixel(x, y);
                    var tranformColor = img.GetPixel(x, y);
                    Assert.AreEqual(etalonColor, tranformColor, $"Color mismatch at {x},{y}: expected '${etalon}', actual '${tranformColor}'");
                }
            }
        }

        [Ignore("For non-automated check")]
        [TestCase(TransformType.RotateCw)]
        [TestCase(TransformType.RotateCcw)]
        [TestCase(TransformType.FlipHorizontal)]
        [TestCase(TransformType.FlipVertical)]
        public async Task ApplyTransformToEtalonImageAndProduceOutputTests(TransformType transformType)
        {
            using (var img = new BitmapImage(TestData.GetZebraImage()))
            {
                img.Transform(transformType);
                var file = Path.Combine(TestData.RootFolder, $"zebra-{transformType}.png");
                await img.SaveAsync(File.Create(file), CancellationToken.None);
            }
        }
    }
}