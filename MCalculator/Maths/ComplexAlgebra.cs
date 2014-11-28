using System.Numerics;

namespace MCalculator.Maths
{
    /// <summary>
    /// Complex Algebratic funtions
    /// </summary>
    public static class ComplexAlgebra
    {
        /// <summary>
        /// Creates a complex number from its standard repesentation
        /// </summary>
        /// <param name="Real">Real part of the number</param>
        /// <param name="Imaginary">imaginary part of the number</param>
        public static Complex ComplexFromRI(double Real, double Imaginary)
        {
            return new Complex(Real, Imaginary);
            }

        /// <summary>
        /// Creates a complex number from its trigonometric representation
        /// </summary>
        /// <param name="r">Absolute value of the number</param>
        /// <param name="angle">The angle of the number</param>
        public static Complex ComplexFromTrigonometric(double r, double angle)
        {
            double Imaginary, Real;
            Real = Trigonometry.Cos(angle) * r;
            Imaginary = Trigonometry.Sin(angle) * r;
            return new Complex(Real, Imaginary);
        }

        /// <summary>
        /// Returns the sine of a complex number
        /// </summary>
        /// <param name="val">input complex number</param>
        public static Complex Sin(Complex val)
        {
            return Complex.Sin(val);
        }

        /// <summary>
        /// Returns the cosine of a complex numbe.
        /// </summary>
        /// <param name="val">input complex number</param>
        public static Complex Cos(Complex val)
        {
            return Complex.Cos(val);
        }

        /// <summary>
        /// Returns the tangent of a complex number
        /// </summary>
        /// <param name="value">input complex number</param>

        public static Complex Tan(Complex value)
        {
            return Complex.Tan(value);
        }

        /// <summary>
        /// Returns the hyperbolic sine of a complex number
        /// </summary>
        /// <param name="val">input complex number</param>
        public static Complex Sinh(Complex val)
        {
            return Complex.Sinh(val);
        }

        /// <summary>
        /// Returns the hyperbolic cosine of a complex number
        /// </summary>
        /// <param name="val">input complex number</param>
        public static Complex Cosh(Complex val)
        {
            return Complex.Cosh(val);
        }

        /// <summary>
        /// Returns the hyperbolic tangent of a complex number
        /// </summary>
        /// <param name="val">Input complex number</param>
        public static Complex Tanh(Complex val)
        {
            return Complex.Tanh(val);
        }

        /// <summary>
        /// Returns the arcus sine of a complex number
        /// </summary>
        /// <param name="val">input complex number</param>
        public static Complex ArcSin(Complex val)
        {
            return Complex.Asin(val);
        }

        /// <summary>
        /// Returns the arcus cosine of a complex number
        /// </summary>
        /// <param name="val">input complex number</param>
        public static Complex ArcCos(Complex val)
        {
            return Complex.Acos(val);
        }

        /// <summary>
        /// Returns the arcus tangent of a complex number
        /// </summary>
        /// <param name="value">input complex number</param>
        public static Complex ArcTan(Complex value)
        {
            return Complex.Atan(value);
        }

        /// <summary>
        /// Gets the absolute value (or magnitude) of a complex number.
        /// </summary>
        /// <param name="value">A complex number.</param>
        public static Complex Abs(Complex value)
        {
            return Complex.Abs(value);
        }

        /// <summary>
        /// Computes the conjugate of a complex number and returns the result.
        /// </summary>
        /// <param name="value">A complex number.</param>
        public static Complex Conjugate(Complex value)
        {
            return Complex.Conjugate(value);
        }

        /// <summary>
        ///  Returns e raised to the power specified by a complex number.
        /// </summary>
        /// <param name="value">The number e raised to the power value.</param>        
        public static Complex Exp(Complex value)
        {
            return Complex.Exp(value);
        }

        /// <summary>
        /// Returns the natural (base e) logarithm of a specified complex number.
        /// </summary>
        /// <param name="value">A complex number.</param>        
        public static Complex Log(Complex value)
        {
            return Complex.Log(value);
        }

        /// <summary>
        ///  Returns the logarithm of a specified complex number in a specified base.
        /// </summary>
        /// <param name="value">A complex number.</param>
        /// <param name="baseValue">The base of the logarithm</param>        
        public static Complex Log(Complex value, double baseValue)
        {
            return Complex.Log(value, baseValue);
        }

        /// <summary>
        /// Returns a specified complex number raised to a power specified by a complex
        /// </summary>
        /// <param name="value">A complex number to be raised to a power.</param>
        /// <param name="power">A complex number that specifies a power.</param>        
        public static Complex Pow(Complex value, Complex power)
        {
            return Complex.Pow(value, power);
        }

        /// <summary>
        /// Returns a specified complex number raised to a power specified by a double-precision floating-point number.
        /// </summary>
        /// <param name="value">A complex number to be raised to a power.</param>
        /// <param name="power">A double-precision floating-point number that specifies a power.</param>        
        public static Complex Pow(Complex value, double power)
        {
            return Complex.Pow(value, power);
        }
    }
}
