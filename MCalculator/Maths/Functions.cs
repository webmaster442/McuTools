using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCalculator.Maths
{
    /// <summary>
    /// General mathematical functions
    /// </summary>
    public static class Functions
    {
        /// <summary>
        /// Replus calculation
        /// </summary>
        /// <param name="x1">Parameter 1</param>
        /// <param name="x2">Parameter 2</param>
        public static double Replus(double x1, double x2)
        {
            return (x1 * x2) / (x1 + x2);
        }

        /// <summary>
        /// Returns the logarithm of a specified number in a specified base.
        /// </summary>
        /// <param name="value1">The number whose logarithm is to be found.</param>
        /// <param name="basen">The base of the logarithm.</param>
        public static double Log(double value1, double basen)
        {
            return Math.Log(value1, basen);
        }

        /// <summary>
        /// Returns the Square root of a specified number
        /// </summary>
        /// <param name="num">the number whose square root is to be found</param>
        public static double Sqrt(double num)
        {
            return Math.Sqrt(num);
        }

        /// <summary>
        /// Returns the root of a specified number in a specified root base
        /// </summary>
        /// <param name="num">the number whose root is to be found</param>
        /// <param name="basen">the root base</param>
        public static double Sqr(double num, double basen)
        {
            return Math.Pow(num, 1 / basen);
        }

        /// <summary>
        /// Returns a specified number raised to the specified power.
        /// </summary>
        /// <param name="num">number to be raised to a power.</param>
        /// <param name="exp">number that specifies a power.</param>
        public static double Pow(double num, double exp)
        {
            return Math.Pow(num, exp);
        }
        
        /// <summary>
        /// finds the remainder of division of one number by another.
        /// </summary>
        /// <param name="num">a number</param>
        /// <param name="div">divisor</param>
        public static double Mod(double num, double div)
        {
            double div2 = Math.Truncate(num / div);
            return num - (div * div2);
        }
        
        /// <summary>
        ///  Calculates a logarithmic unit that indicates the ratio of a physical quantity (usually power or intensity) relative to a specified or implied reference level.
        /// </summary>
        /// <param name="x0">Input number 1</param>
        /// <param name="x">Input number 2</param>
        public static double Db10(double x0, double x)
        {
            return 10 * Math.Log10(x / x0);
        }

        /// <summary>
        ///  Calculates a logarithmic unit that indicates the ratio of a physical quantity (usually power or intensity) relative to a specified or implied reference level.
        /// </summary>
        /// <param name="x0">Input number 1</param>
        /// <param name="x">Input number 2</param>
        public static double Db20(double x0, double x)
        {
            return 20 * Math.Log10(x / x0);
        }

        /// <summary>
        /// Returns the least common multiple of two numbers
        /// </summary>
        /// <param name="x">A number</param>
        /// <param name="y">Another number</param>
        /// <returns></returns>
        public static double Lcm(double x, double y)
        {
            return Math.Round((x * y) / Gcd(x, y), 0);
        }

        /// <summary>
        /// Returns the Greatest common divisor of two numbers
        /// </summary>
        /// <param name="x">A number</param>
        /// <param name="y">Another number</param>
        public static double Gcd(double x, double y) //LNKO
        {
            if ((x == 0) || (y == 0)) throw new ArgumentException("Can't divide with zero!");
            while (x != y)
            {
                if (x > y) x = x - y;
                else y = y - x;
            }
            return x;
        }

        /// <summary>
        /// Checks if the parameter double number is only integer or not
        /// </summary>
        /// <param name="number">The number to test</param>
        public static bool IsInteger(double number)
        {
            return (number - Math.Truncate(number)) == 0;
        }
    }
}
