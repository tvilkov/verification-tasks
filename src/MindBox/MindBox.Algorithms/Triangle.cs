using System;

namespace MindBox
{
    /// <summary>
    /// Triangle representation
    /// </summary>
    /// <inheritdoc cref="IFigure"/>
    public class Triangle : IFigure
    {
        /// <summary>
        /// Creates new instance of the class
        /// </summary>
        /// <param name="a">First side</param>
        /// <param name="b">Second side</param>
        /// <param name="c">Third side</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if any of the side is not a positive number</exception>
        /// <exception cref="InvalidOperationException">Thrown if triangle with given sides doesn't exist</exception>
        public Triangle(double a, double b, double c)
        {
            if (a <= 0) throw new ArgumentOutOfRangeException(nameof(a), "Expected to be a positive number");
            if (b <= 0) throw new ArgumentOutOfRangeException(nameof(b), "Expected to be a positive number");
            if (c <= 0) throw new ArgumentOutOfRangeException(nameof(c), "Expected to be a positive number");
            if (!Algorithms.IsTriangleExists(a, b, c)) throw new InvalidOperationException("A triangle with given sides doesn't exist");
            A = a;
            B = b;
            C = c;
            IsRightTriangle = Algorithms.IsRightTriangle(a, b, c);
        }

        /// <summary>
        /// Returns length of the first side
        /// </summary>
        public double A { get; }

        /// <summary>
        /// Returns length of the second side
        /// </summary>
        public double B { get; }

        /// <summary>
        /// Returns length of the third side
        /// </summary>
        public double C { get; }

        /// <summary>
        /// Returns <c>true</c> if given triangle is a right one (has 90 degree angle)
        /// </summary>
        public bool IsRightTriangle { get; }

        public double CalculateArea() => Algorithms.CalculateTriangleArea(A, B, C);

        /// <summary>
        /// Returns a string that represents current triangle
        /// </summary>        
        public override string ToString()
        {
            return $"Triangle (a={A}, b={B}, c={C})";
        }
    }
}