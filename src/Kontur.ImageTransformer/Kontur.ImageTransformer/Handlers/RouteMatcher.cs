using System;
using Kontur.ImageTransformer.Drawing;
using Kontur.ImageTransformer.Util;

namespace Kontur.ImageTransformer.Handlers
{
    internal static class RouteMatcher
    {
        public static bool TryMatchImageFilterRequest(string httpMethod, Uri requestUri, out string filterDefinition, out Coordinates coords)
        {
            // Format: POST /process/<filter>/x,y,w,h

            filterDefinition = null;
            coords = Coordinates.Empty;
            if (!string.Equals(httpMethod, "POST", StringComparison.OrdinalIgnoreCase)) return false;

            var segments = requestUri.AbsolutePath.Split(Delimiters.Slashes, StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length != 3 || !string.Equals(segments[0], "process", StringComparison.OrdinalIgnoreCase)) return false;

            filterDefinition = segments[1];

            return Coordinates.TryParse(segments[2], out coords);
        }

        public static bool TryMatchImageTransformRequest(string httpMethod, Uri requestUri, out TransformType transformType, out Coordinates coords)
        {
            // Format: POST /process/rotate-cw|rotate-ccw|flip-h|flip-w/x,y,w,h

            if (httpMethod == null) throw new ArgumentNullException(nameof(httpMethod));
            if (requestUri == null) throw new ArgumentNullException(nameof(requestUri));            

            transformType = TransformType.None;
            coords = Coordinates.Empty;

            if (!string.Equals(httpMethod, "POST", StringComparison.OrdinalIgnoreCase)) return false;

            var segments = requestUri.AbsolutePath.Split(Delimiters.Slashes, StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length != 3 || !string.Equals(segments[0], "process", StringComparison.OrdinalIgnoreCase)) return false;

            return tryMatchTransform(segments[1], out transformType) && Coordinates.TryParse(segments[2], out coords);
        }

        private static bool tryMatchTransform(string transformDefinition, out TransformType transformType)
        {
            if (string.Equals("rotate-cw", transformDefinition, StringComparison.CurrentCultureIgnoreCase))
            {
                transformType = TransformType.RotateCw;
                return true;
            }

            if (string.Equals("rotate-ccw", transformDefinition, StringComparison.CurrentCultureIgnoreCase))
            {
                transformType = TransformType.RotateCcw;
                return true;
            }

            if (string.Equals("flip-h", transformDefinition, StringComparison.CurrentCultureIgnoreCase))
            {
                transformType = TransformType.FlipHorizontal;
                return true;
            }

            if (string.Equals("flip-v", transformDefinition, StringComparison.CurrentCultureIgnoreCase))
            {
                transformType = TransformType.FlipVertical;
                return true;
            }

            transformType = TransformType.None;
            return false;
        }
    }
}