using System.IO;

namespace Kontur.ImageTransformer.Drawing
{
    internal interface IImageFactory
    {
        IImage CreateFrom(Stream source);
    }
}