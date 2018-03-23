using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Kontur.ImageTransformer.Drawing
{
    internal interface IImage : IDisposable
    {
        int Width { get; }
        int Height { get; }
        Rectangle Bounds { get; }
        void SetPixel(int x, int y, Color color);
        Color GetPixel(int x, int y);
        IImage Copy(Rectangle rect);
        Task SaveAsync(Stream dest, CancellationToken cancellationToken = default(CancellationToken));
        void Transform(TransformType transformType);
    }

    internal enum TransformType
    {
        None = 0,
        RotateCw = 1,
        RotateCcw = 2,
        FlipHorizontal = 3,
        FlipVertical = 4
    }
}