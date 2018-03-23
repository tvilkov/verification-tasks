using System.Drawing;
using System.IO;
using Kontur.ImageTransformer.Drawing;

// ReSharper disable once CheckNamespace
namespace Kontur.ImageTransformer.Tests
{
    internal static class TestData
    {
        private static Bitmap loadBitmap(string name)
        {
            var path = Path.Combine(RootFolder, "32bit", name);
            var image = Image.FromFile(path, true);
            return new Bitmap(image);
        }

        public static string RootFolder => Path.Combine(Path.GetDirectoryName(typeof(TestData).Assembly.Location), "TestData");

        public static Bitmap GetTestImage(int w, int h)
        {
            return loadBitmap($"{w}x{h}.png");
        }

        public static Bitmap GetZebraImage(TransformType transformType = TransformType.None)
        {
            if (transformType == TransformType.None)
            {
                return loadBitmap("zebra.png");
            }

            return loadBitmap($"zebra-{transformType}.png");
        }
    }
}