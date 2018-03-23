using System;
using System.Drawing;
using Kontur.ImageTransformer.Drawing;

namespace Kontur.ImageTransformer.Filters
{
    internal class SepiaFilter : ImageFilter
    {
        protected override Color CalculatePixelColor(IImage source, int x, int y)
        {
            // TODO[tv]: PERF can't this be replaced with Sum(int*1000) / 1000?
            var color = source.GetPixel(x, y);
            var r = (int) Math.Floor(color.R * 0.393 + color.G * 0.769 + color.B * 0.189);
            var g = (int) Math.Floor(color.R * 0.349 + color.G * 0.686 + color.B * 0.168);
            var b = (int) Math.Floor(color.R * 0.272 + color.G * 0.534 + color.B * 0.131);
            return Color.FromArgb(color.A, Math.Min(255, r), Math.Min(255, g), Math.Min(255, b));
        }
    }
}