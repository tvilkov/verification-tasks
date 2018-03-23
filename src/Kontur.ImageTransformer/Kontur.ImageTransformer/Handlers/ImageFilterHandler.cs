using System;
using System.Drawing;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Kontur.ImageTransformer.Drawing;
using Kontur.ImageTransformer.Filters;
using Kontur.ImageTransformer.Util;

namespace Kontur.ImageTransformer.Handlers
{
    internal class ImageFilterHandler : IRequestHandler
    {
        private readonly FiltersRegistry m_Filters;
        private readonly IImageFactory m_ImageFactory;

        public ImageFilterHandler(FiltersRegistry filters, IImageFactory imageFactory)
        {
            m_Filters = filters ?? throw new ArgumentNullException(nameof(filters));
            m_ImageFactory = imageFactory ?? throw new ArgumentNullException(nameof(imageFactory));
        }

        public async Task<bool> Handle(HttpListenerContext context, CancellationToken cancellationToken)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            cancellationToken.ThrowIfCancellationRequested();

            if (!RouteMatcher.TryMatchImageFilterRequest(context.Request.HttpMethod, context.Request.Url, out var filterDefinition, out var coords))
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

            // Ensure filter is known
            var filter = m_Filters.GetFilterByDefinition(filterDefinition);
            if (filter == null)
            {
                context.SendBadRequest($"Unknown filter '{filterDefinition}'");
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
                    context.SendBadRequest("Image size to big");
                    return true;
                }

                cancellationToken.ThrowIfCancellationRequested(); // Check after long running operation

                // Find intersection area                
                var filterBounds = Rectangle.Intersect(image.Bounds, coords.ToRectangle());
                if (filterBounds.IsEmpty)
                {
                    context.SendNoContent();
                    return true;
                }

                cancellationToken.ThrowIfCancellationRequested(); // Check before starting long-running operation 

                await filter.Apply(image, filterBounds, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested(); // Check after long running operation and before another one

                using (var copy = image.Copy(filterBounds))
                {
                    context.Response.AddHeader("Content-Type", "image/png");
                    await copy.SaveAsync(context.Response.OutputStream, cancellationToken);
                    context.Response.Close();
                }

                return true;
            }
        }
    }

}