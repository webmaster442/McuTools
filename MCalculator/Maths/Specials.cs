using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Windows;

namespace MCalculator.Maths
{
    /// <summary>
    /// One Parameter function prototype
    /// </summary>
    /// <param name="param">A function parameter</param>
    public delegate double OneParamFunction(double param);

    /// <summary>
    /// Special mathematical functions
    /// </summary>
    public static class Specials
    {
        /// <summary>
        /// Approximation of the definite integral of an analytic smooth function on a closed interval.
        /// </summary>
        /// <param name="f">The analytic smooth function to integrate.</param>
        /// <param name="intervalBegin">Where the interval starts, inclusive and finite.</param>
        /// <param name="intervalEnd">Where the interval stops, inclusive and finite.</param>
        public static double Integrate(Func<double, double> f, double intervalBegin, double intervalEnd)
        {
            return MathNet.Numerics.Integrate.OnClosedInterval(f, intervalBegin, intervalEnd);
        }

        /// <summary>
        /// Solves a System of Linear equations in the form of Ax = b
        /// </summary>
        /// <param name="Ax">A matrix containing the coefficients of the variables</param>
        /// <param name="b">A set of constants</param>
        public static Set SolveEquationSystem(DenseMatrix Ax, Set b)
        {
            DenseMatrix inverse = (DenseMatrix)Ax.Inverse();
            DenseVector vect = Set.ToVector(b);
            var result = inverse * vect;
            return Set.FromVector(result);
        }

        /// <summary>
        /// Creates a Fraction number
        /// </summary>
        /// <param name="num">Numerator</param>
        /// <param name="denom">Denominator</param>
        public static Fraction Fraction(long num, long denom)
        {
            return new Fraction(num, denom);
        }

        /// <summary>
        /// Creates a Fraction number from a double value
        /// </summary>
        /// <param name="value">A double value</param>
        public static Fraction Fraction(double value)
        {
            return new Fraction(value);
        }
    }
}