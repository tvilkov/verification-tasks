using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Kontur.ImageTransformer.Drawing;
using Kontur.ImageTransformer.Util;

namespace Kontur.ImageTransformer.Filters
{
    internal abstract class ImageFilter
    {
        public Task Apply(IImage source, Rectangle filterBounds, CancellationToken cancellationToken)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            for (var y = 0; y < filterBounds.Height; y++)
            {
                for (var x = 0; x < filterBounds.Width; x++)
                {
                    var newColor = CalculatePixelColor(source, x, y);
                    source.SetPixel(x, y, newColor);
                }

                if (y % 10 == 0) // PERF: Check for cancellation every 10 lines
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
            }

            return Empty.Task;
        }

        protected abstract Color CalculatePixelColor(IImage source, int x, int y);
    }
}