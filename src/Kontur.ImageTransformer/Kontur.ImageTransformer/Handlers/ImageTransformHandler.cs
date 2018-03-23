using System;
using System.Drawing;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Kontur.ImageTransformer.Drawing;
using Kontur.ImageTransformer.Util;

namespace Kontur.ImageTransformer.Handlers
{
    internal class ImageTransformHandler : IRequestHandler
    {
        private readonly IImageFactory m_ImageFactory;

        public ImageTransformHandler(IImageFactory imageFactory)
        {
            m_ImageFactory = imageFactory ?? throw new ArgumentNullException(nameof(imageFactory));
        }

        public async Task<bool> Handle(HttpListenerContext context, CancellationToken cancellationToken)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            cancellationToken.ThrowIfCancellationRequested();

            if (!RouteMatcher.TryMatchImageTransformRequest(context.Request.HttpMethod, context.Request.Url, out var transformType, out var coords))
            {
                // Not our route - return false, to allow proceed to the next handler (if any)
                return false;
            }

            // Ensure we have content
            if (context.Request.ContentLength64 <= 0)
            {
                context.SendBadRequest("Image is absent");
                return true;
            }

            // Ensure it's of valid size
            if (context.Request.ContentLength64 > Constants.MAX_IMAGE_SIZE)
            {
                context.SendBadRequest("Image is to big (over 100Kb)");
                return true;
            }

            cancellationToken.ThrowIfCancellationRequested(); // Check before starting long-running operation

            // Try read image payload            
            IImage image;
            try
            {
                image = m_ImageFactory.CreateFrom(context.Request.InputStream);
            }
            catch (Exception)
            {
                context.SendBadRequest("Bad image format");
                return true;
            }

            using (image)
            {
                // Check image size
                if (image.Width > Constants.MAX_IMAGE_WIDTH || image.Height > Constants.MAX_IMAGE_HEIGHT)
                {
                    context.SendBadRequest("Image size is to big (over 1000px in width, height or both)");
                    return true;
                }

                cancellationToken.ThrowIfCancellationRequested(); // Check before starting long-running operation 

                image.Transform(transformType);

                cancellationToken.ThrowIfCancellationRequested(); // Check after long running operation and before another one

                // Find clipping area                
                var clipBounds = Rectangle.Intersect(image.Bounds, coords.ToRectangle());
                if (clipBounds.IsEmpty)
                {
                    context.SendNoContent();
                    return true;
                }

                // Produce output
                context.Response.Headers.Add("Content-Type", "image/png");
                if (clipBounds == image.Bounds)
                {
                    // No clipping required - save image as is
                    await image.SaveAsync(context.Response.OutputStream, cancellationToken);
                }
                else
                {
                    using (var clippedImage = image.Copy(clipBounds))
                    {
                        await clippedImage.SaveAsync(context.Response.OutputStream, cancellationToken);
                    }
                }

                context.Response.Close();

                return true;
            }
        }
    }
}