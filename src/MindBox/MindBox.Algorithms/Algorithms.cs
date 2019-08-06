using System;

namespace MindBox
{
    /// <summary>
    /// Holds low-level geometry algorithms.
    /// Note[tv]: not publically exposed at the moment.
    /// </summary>
    internal static class Algorithms
    {
        private static double m_Epsilon = 1e-10;

        /// <summary>
        /// Gets or sets epsilon values for floating point operations (equality checks).
        /// Note: value affects all algorithms withing Algorithms module.
        /// </summary>
        public static double Epsilon
        {
            get { return m_Epsilon; }
            set
            {
                if (value <= 0) throw new InvalidOperationException($"{nameof(Epsilon)}: must be positive value");
                if (value > 0.001) throw new InvalidOperationException($"{nameof(Epsilon)}: the value can't be greater than 0.001");
                m_Epsilon = value;
            }
        }

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
            if (sideA <= 0) throw new ArgumentException("sideA: length expected to be a positive value", nameof(sideA));
            if (sideB <= 0) throw new ArgumentException("sideB: length expected to be a positive value", nameof(sideB));
            if (sideC <= 0) throw new ArgumentException("sideC: length expected to be a positive value", nameof(sideC));

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

            if (!IsTriangleExists(sideA, sideB, sideC)) return false;

            // Note[tv]: Heron formula is used, refer to http://ru.wikipedia.org/wiki/Формула_Герона
            var p = (sideA + sideB + sideC) / 2;

            area = Math.Sqrt(p * (p - sideA) * (p - sideB) * (p - sideC));

            return true;
        }

        /// <summary>
        /// Checks if triangle with sides <paramref name="sideA"/>, <paramref name="sideB"/>, <paramref name="sideC"/> actually exists.
        /// </summary>        
        /// <param name="sideA">The length of the first side.</param>
        /// <param name="sideB">The length of the second side.</param>
        /// <param name="sideC">The length of the third side.</param>
        /// <returns>Returns <c>true</c> if triangle exists, <c>false</c> otherwise</returns>
        public static bool IsTriangleExists(double sideA, double sideB, double sideC)
        {
            if (sideA <= 0 || sideB <= 0 || sideC <= 0) return false;

            // Invariant: length of any trinagle's side is less then total length of two other sides.
            if (sideA + sideB <= sideC || sideA + sideC <= sideB || sideB + sideC <= sideA) return false;

            return true;
        }

        /// <summary>
        /// Checks if triangle with sides <paramref name="sideA"/>, <paramref name="sideB"/>, <paramref name="sideC"/> is a right one.
        /// A triangle considered 'right' if one of it's angles is 90 degrees.
        /// </summary>        
        /// <param name="sideA">The length of the first side.</param>
        /// <param name="sideB">The length of the second side.</param>
        /// <param name="sideC">The length of the third side.</param>        
        /// <returns>Returns <c>true</c> if triangle is a right one, <c>false</c> otherwise</returns>
        public static bool IsRightTriangle(double sideA, double sideB, double sideC)
        {
            if (!IsTriangleExists(sideA, sideB, sideC)) return false;

            // Sort sides so that sideC contains max of them (hypotenuse)
            double tmp;

            if (sideA > sideB)
            {
                tmp = sideA;
                sideA = sideB;
                sideB = tmp;
            }

            if (sideB > sideC)
            {
                tmp = sideB;
                sideB = sideC;
                sideC = tmp;
            }

            // Note[tv]: use Pythagoras' theorem for detection
            return Math.Abs(sideA * sideA + sideB * sideB - sideC * sideC) < Epsilon;
        }
    }
}