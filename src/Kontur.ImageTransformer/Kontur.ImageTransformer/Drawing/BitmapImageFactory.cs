using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Kontur.ImageTransformer.Drawing
{
    internal class BitmapImageFactory : IImageFactory
    {
        public virtual IImage CreateFrom(Stream source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var image = Image.FromStream(source, true);

            if (!ImageFormat.Png.Equals(image.RawFormat))
            {
                throw new BadImageFormatException("PNG image expected");
            }

            var bitmap = new Bitmap(image);
            return new BitmapImage(bitmap);
        }
    }
}