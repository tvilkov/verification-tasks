using System;

namespace MindBox
{
    public static class Algorithms
    {
        /// <summary>
        /// Calculates an area of the triangle defined by sides <paramref name="sideA"/>, <paramref name="sideB"/>, <paramref name="sideC"/>.
        /// </summary>
        /// <param name="sideA">The length of the first side.</param>
        /// <param name="sideB">The length of the second side.</param>
        /// <param name="sideC">The length of the third side.</param>
        /// <returns>Calculated area</returns>
        /// <remarks>If given triangle doesn't exist, throws <see cref="T:System.InvalidOperationException"/>.</remarks>
        public static double CalculateTriangleArea(double sideA, double sideB, double sideC)
        {
            if (sideA <= 0) throw new ArgumentException("sideA: length expected to be a positive value", "sideA");
            if (sideB <= 0) throw new ArgumentException("sideB: length expected to be a positive value", "sideB");
            if (sideC <= 0) throw new ArgumentException("sideC: length expected to be a positive value", "sideC");

            double area;
            if (!TryCalculateTriangleArea(sideA, sideB, sideC, out area)) throw new InvalidOperationException("Triangle doesn't exist");

            return area;
        }

        /// <summary>
        /// Safelly tries to calculate an area of the triangle defined by sides <paramref name="sideA"/>, <paramref name="sideB"/>, <paramref name="sideC"/>.
        /// </summary>
        /// <param name="sideA">The length of the first side.</param>
        /// <param name="sideB">The length of the second side.</param>
        /// <param name="sideC">The length of the third side.</param>
        /// <param name="area">Calculated area length</param>
        /// <returns>Returns <c>true</c> if calculation succeded, <c>false</c> - if didn't.</returns>
        public static bool TryCalculateTriangleArea(double sideA, double sideB, double sideC, out double area)
        {
            area = 0;

            if (!isTriangleExists(sideA, sideB, sideC)) return false;

            // Note[tv]: Heron formula is used, refer to http://ru.wikipedia.org/wiki/Формула_Герона
            var p = (sideA + sideB + sideC) / 2;

            area = Math.Sqrt(p * (p - sideA) * (p - sideB) * (p - sideC));

            return true;
        }

        private static bool isTriangleExists(double a, double b, double c)
        {
            if (a <= 0 || b <= 0 || c <= 0) return false;

            // Invariant: length of any trinagle's side is less then total length of two other sides.
            if (a + b <= c || a + c <= b || b + c <= a) return false;

            return true;
        }
    }
}