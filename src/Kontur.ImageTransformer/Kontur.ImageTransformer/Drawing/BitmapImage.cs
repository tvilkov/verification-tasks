using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Kontur.ImageTransformer.Util;

namespace Kontur.ImageTransformer.Drawing
{
    internal class BitmapImage : IImage
    {
        private Bitmap m_Bitmap;
        private BitmapData m_BitmapData;
        private readonly int m_BytesPerPixel;

        public BitmapImage(Bitmap bitmap)
        {
            m_Bitmap = bitmap ?? throw new ArgumentNullException(nameof(bitmap));
            var colorDepth = Image.GetPixelFormatSize(m_Bitmap.PixelFormat);
            if (colorDepth != 8 && colorDepth != 24 && colorDepth != 32)
            {
                throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");
            }

            m_BytesPerPixel = colorDepth / 8;
            lockBits();
        }

        public int Width => m_Bitmap.Width;

        public int Height => m_Bitmap.Height;

        public Rectangle Bounds => new Rectangle(0, 0, Width, Height);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void SetPixel(int x, int y, Color color)
        {
            var pixel = (byte*) m_BitmapData.Scan0 + y * m_BitmapData.Stride + x * m_BytesPerPixel;

            if (m_BytesPerPixel == 4) // For 32 bpp - Red, Green, Blue and Alpha
            {
                pixel[0] = color.B;
                pixel[1] = color.G;
                pixel[2] = color.R;
                pixel[3] = color.A;
            }

            if (m_BytesPerPixel == 3) // For 24 bpp - Red, Green and Blue
            {
                pixel[0] = color.B;
                pixel[1] = color.G;
                pixel[2] = color.R;
            }

            if (m_BytesPerPixel == 1) // For 8 bpp all color values are the same
            {
                pixel[0] = color.B;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe Color GetPixel(int x, int y)
        {
            // Get pointer to in memory pixel location
            var pixel = (byte*) m_BitmapData.Scan0 + y * m_BitmapData.Stride + x * m_BytesPerPixel;

            if (m_BytesPerPixel == 4) // For 32 bpp - Red, Green, Blue and Alpha
            {
                var b = pixel[0];
                var g = pixel[1];
                var r = pixel[2];
                var a = pixel[3];
                return Color.FromArgb(a, r, g, b);
            }

            if (m_BytesPerPixel == 3) // For 24 bpp get Red, Green and Blue
            {
                var b = pixel[0];
                var g = pixel[1];
                var r = pixel[2];
                return Color.FromArgb(r, g, b);
            }

            if (m_BytesPerPixel == 1) // For 8 bpp all color values are the same
            {
                var c = pixel[0];
                return Color.FromArgb(c, c, c);
            }

            return Color.Empty;
        }

        public IImage Copy(Rectangle rect)
        {
            // TODO: PERF how quick is Clone method?            
            var copy = m_Bitmap.Clone(rect, m_BitmapData.PixelFormat);
            return new BitmapImage(copy);
        }

        public Task SaveAsync(Stream dest, CancellationToken cancellationToken)
        {
            if (dest == null) throw new ArgumentNullException(nameof(dest));
            m_Bitmap.Save(dest, ImageFormat.Png);
            return Empty.Task;
        }

        public void Transform(TransformType transformType)
        {
            RotateFlipType rfType;

            switch (transformType)
            {
                case TransformType.None:
                    return;
                case TransformType.RotateCw:
                    rfType = RotateFlipType.Rotate90FlipNone;
                    break;
                case TransformType.RotateCcw:
                    rfType = RotateFlipType.Rotate270FlipNone;
                    break;
                case TransformType.FlipHorizontal:
                    rfType = RotateFlipType.RotateNoneFlipX;
                    break;
                case TransformType.FlipVertical:
                    rfType = RotateFlipType.RotateNoneFlipY;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            unlockBits();
            m_Bitmap.RotateFlip(rfType);
            lockBits();
        }

        public void Dispose()
        {
            if (m_Bitmap == null) return;
            unlockBits();
            m_Bitmap.Dispose();
            m_Bitmap = null;
        }

        private void lockBits()
        {
            var bounds = new Rectangle(0, 0, Width, Height);
            m_BitmapData = m_Bitmap.LockBits(bounds, ImageLockMode.ReadWrite, m_Bitmap.PixelFormat);
        }

        private void unlockBits()
        {
            if (m_BitmapData == null) return;
            m_Bitmap.UnlockBits(m_BitmapData);
            m_BitmapData = null;
        }
    }
}