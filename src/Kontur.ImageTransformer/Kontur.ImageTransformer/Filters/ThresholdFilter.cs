using System;
using System.Drawing;
using Kontur.ImageTransformer.Drawing;

namespace Kontur.ImageTransformer.Filters
{
    internal class ThresholdFilter : ImageFilter
    {
        private readonly int m_Threthold;

        public ThresholdFilter(int threthold)
        {
            if (threthold < 0 || threthold > 100) throw new ArgumentOutOfRangeException(nameof(threthold), "Expected value in range [0,100]");
            m_Threthold = threthold;
        }

        protected override Color CalculatePixelColor(IImage source, int x, int y)
        {
            var color = source.GetPixel(x, y);
            var intensity = (color.R + color.G + color.B) / 3;
            return intensity > 255 * m_Threthold / 100 ? Color.FromArgb(color.A, 255, 255, 255) : Color.FromArgb(color.A, 0, 0, 0);
        }
    }
}