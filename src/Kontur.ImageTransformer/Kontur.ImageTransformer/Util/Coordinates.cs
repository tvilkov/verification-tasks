using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;

namespace Kontur.ImageTransformer.Util
{
    [DebuggerDisplay("{" + nameof(ToString) + "()}")]
    internal struct Coordinates
    {
        public readonly int X;
        public readonly int Y;
        public readonly int W;
        public readonly int H;

        public Coordinates(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }

        public static readonly Coordinates Empty = new Coordinates(0, 0, 0, 0);

        public bool Equals(Coordinates other)
        {
            return X == other.X && Y == other.Y && W == other.W && H == other.H;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Coordinates coordindates && Equals(coordindates);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ W;
                hashCode = (hashCode * 397) ^ H;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"Coordinates (X={X},Y={Y},W={W},H={H})";
        }

        public Rectangle ToRectangle()
        {
            if (W >= 0 && H >= 0)
            {
                return new Rectangle(X, Y, W, H);
            }

            var x = X;
            var y = Y;

            if (W < 0)
            {
                x += W;
            }

            if (H < 0)
            {
                y += H;
            }

            return new Rectangle(x, y, Math.Abs(W), Math.Abs(H));
        }

        public static bool TryParse(string input, out Coordinates coords)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            coords = Empty;

            // Fail fast: expected format is x,y,w,h, so min len is 7 chars
            if (input.Length < 7) return false;

            var parts = input.Split(Delimiters.Comma);

            if (parts.Length == 4 &&
                int.TryParse(parts[0], NumberStyles.Number, CultureInfo.InvariantCulture, out var x) &&
                int.TryParse(parts[1], NumberStyles.Number, CultureInfo.InvariantCulture, out var y) &&
                int.TryParse(parts[2], NumberStyles.Number, CultureInfo.InvariantCulture, out var w) &&
                int.TryParse(parts[3], NumberStyles.Number, CultureInfo.InvariantCulture, out var h))
            {
                coords = new Coordinates(x, y, w, h);
                return true;
            }

            return false;
        }
    }
}