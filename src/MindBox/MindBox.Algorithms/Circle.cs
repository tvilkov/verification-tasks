using System;
using System.Globalization;

namespace MindBox
{
    /// <summary>
    /// Circle representation
    /// </summary>
    /// <inheritdoc cref="IFigure"/>
    public class Circle : IFigure
    {
        /// <summary>
        /// Creates new instance of the class
        /// </summary>
        /// <param name="radius">Radius of the circle</param>        
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="radius"/> is not a positive number</exception>        
        public Circle(double radius)
        {
            if (radius <= 0) throw new ArgumentOutOfRangeException(nameof(radius), "Radius is expected to be a positive number");
            Radius = radius;
        }

        /// <summary>
        /// Returns radius of the circle
        /// </summary>
        public double Radius { get; }

        public double CalculateArea() => Math.PI * Radius * Radius;

        /// <summary>
        /// Returns a string that represents current circle
        /// </summary>        
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Circle (Radius={0})", Radius);
        }
    }
}