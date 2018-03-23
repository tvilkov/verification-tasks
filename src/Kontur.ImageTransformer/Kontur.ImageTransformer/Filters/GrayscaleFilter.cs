using System;
using System.Drawing;
using Kontur.ImageTransformer.Drawing;

namespace Kontur.ImageTransformer.Filters
{
    internal class GrayscaleFilter : ImageFilter
    {
        protected override Color CalculatePixelColor(IImage source, int x, int y)
        {
            var color = source.GetPixel(x, y);
            var intensity = Math.Min(255, (color.R + color.G + color.B) / 3);
            return Color.FromArgb(color.A, intensity, intensity, intensity);
        }
    }
}