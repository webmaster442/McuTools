using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace MCalculator.Maths
{
    /// <summary>
    /// Random number generation functions
    /// </summary>
    public static class Rnd
    {
        static Random rnd = new Random();
        static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        /// <summary>
        /// Generates a random integer number in the range of minimum and maximum
        /// </summary>
        /// <param name="minimum">Minimum value</param>
        /// <param name="maximum">Maximum value</param>
        public static double Random(int minimum, int maximum)
        {
            return rnd.Next(minimum, maximum);
        }

        /// <summary>
        /// Generates a random double number in the range of minimum and maximum
        /// </summary>
        /// <param name="minimum">Minimum value</param>
        /// <param name="maximum">Maximum value</param>
        public static double Random(double minimum, double maximum)
        {
            double rand = 0;
            do
            {
                rand = rnd.Next((int)Math.Truncate(minimum), (int)Math.Truncate(maximum)) + rnd.NextDouble();
            }
            while (rand > maximum || rand < minimum);
            return rand;
        }

        /// <summary>
        /// Generates a random number in the range of 0 and Int.MaxValue
        /// </summary>
        public static double Random()
        {
            return rnd.Next(int.MaxValue);
        }

        /// <summary>
        /// Generates a random floating point number in the range of 0 and 1
        /// </summary>
        /// <returns></returns>
        public static double RandomFloat()
        {
            return rnd.NextDouble();
        }

        /// <summary>
        /// Geneatates a Cryotographic random number
        /// </summary>
        /// <param name="bytes">The number of random bytes to use, must be in range of 1 and 8</param>
        /// <param name="signed">specifies that the number can be negative or not. 1 - can be negative, 0 - can't be negative</param>
        public static double CriptoRand(int bytes, bool signed)
        {
            byte[] tmp = null;
            if (bytes < 1 || bytes > 8) throw new ArgumentException("Byte count must be at least 1 and maximum 8");

            switch (bytes)
            {
                case 1:
                    tmp = new byte[bytes];
                    rng.GetBytes(tmp);
                    if (signed) return (sbyte)tmp[0];
                    else return tmp[0];
                case 2:
                    tmp = new byte[bytes];
                    rng.GetBytes(tmp);
                    if (signed) return BitConverter.ToInt16(tmp, 0);
                    else return BitConverter.ToUInt16(tmp, 0);
                case 3:
                    tmp = new byte[4];
                    rng.GetBytes(tmp);
                    if (signed) return BitConverter.ToInt32(tmp, 0) & 0x80FFFFFF;
                    else return BitConverter.ToUInt32(tmp, 0) & 0x00FFFFFF;
                case 4:
                    if (signed) return BitConverter.ToInt32(tmp, 0);
                    else return BitConverter.ToUInt32(tmp, 0);
                case 5:
                    tmp = new byte[8];
                    rng.GetBytes(tmp);
                    if (signed) return BitConverter.ToInt64(tmp, 0) & 0x000000FFFFFFFFFF;
                    else return BitConverter.ToUInt64(tmp, 0) & 0x000000FFFFFFFFFF;
                case 6:
                    tmp = new byte[8];
                    rng.GetBytes(tmp);
                    if (signed) return BitConverter.ToInt64(tmp, 0) & 0x0000FFFFFFFFFFFF;
                    else return BitConverter.ToUInt64(tmp, 0) & 0x0000FFFFFFFFFFFF;
                case 7:
                    tmp = new byte[8];
                    rng.GetBytes(tmp);
                    if (signed) return BitConverter.ToInt64(tmp, 0) & 0x00FFFFFFFFFFFFFF;
                    else return BitConverter.ToUInt64(tmp, 0) & 0x00FFFFFFFFFFFFFF;
                case 8:
                    tmp = new byte[8];
                    rng.GetBytes(tmp);
                    if (signed) return BitConverter.ToInt64(tmp, 0);
                    else return BitConverter.ToUInt64(tmp, 0);
                default:
                    return double.NaN;
            }
        }

        /// <summary>
        /// Geneatates a Cryotographic random float number. The number is never NaN, or Infinity
        /// </summary>
        /// <returns></returns>
        public static double CryptoFloatRand()
        {
            double res = double.NaN;
            do
            {
                byte[] arr = new byte[8];
                rng.GetBytes(arr);
                res = BitConverter.ToDouble(arr, 0);
            }
            while (double.IsNaN(res) || res == double.NegativeInfinity || res == double.PositiveInfinity);
            return res;
        }

        /// <summary>
        /// Creates a set of random numbers
        /// </summary>
        /// <param name="items">Number of numbers to generate</param>
        /// <param name="minimum">Minimum number value</param>
        /// <param name="maximum">Maximum number value</param>
        public static Set CreateSeries(int items, double minimum, double maximum)
        {
            Set ret = new Set(items);
            for (int i = 0; i < ret.Count; i++)
            {
                if (Functions.IsInteger(minimum) && Functions.IsInteger(maximum))
                {
                    ret[i] = Random((int)minimum, (int)maximum);
                }
                else ret[i] = Random(minimum, maximum);
            }
            return ret;
        }
    }
}
