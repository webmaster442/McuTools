using System;
using System.Collections.Generic;
using System.Text;

namespace MCalculator.Maths
{
    /// <summary>
    /// Number information class
    /// </summary>
    public class NumberInfo
    {
        private string ByteArrayToString(byte[] input, string format, int system = 10)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in input)
            {
                if (system == 2 || system == 8 || system == 16) sb.AppendFormat(format, Convert.ToString(item, system));
                else sb.AppendFormat(format, item);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Empty constructor
        /// </summary>
        public NumberInfo() { }

        /// <summary>
        /// Returns true, if the tested number is greater than 0
        /// </summary>
        public bool IsPositive { get; private set; }
        
        /// <summary>
        /// Returns true, if the tested number is negative
        /// </summary>
        public bool IsNegative { get; private set; }
        
        /// <summary>
        /// Returns true, if the tested number is an integer
        /// </summary>
        public bool IsInteger { get; private set; }

        /// <summary>
        /// Returns true, if the tested number is a floating point number
        /// </summary>
        public bool IsFloat { get; private set; }

        /// <summary>
        /// Returns true, if the number is floating point and it's not a number
        /// </summary>
        public bool IsNaN { get; private set; }
        
        /// <summary>
        /// Returns true, if the number is floating point and it's positive infinity
        /// </summary>
        public bool IsPositiveInfinity { get; private set; }

        /// <summary>
        /// Returns true, if the number is floating point and it's negative infinity
        /// </summary>
        public bool IsNegativeInfinity { get; private set; }
        
        /// <summary>
        /// Returns the number of bits required to represent the number. If the number is floating point 64 will be returned
        /// </summary>
        public int BitLength { get; private set; }
        
        /// <summary>
        /// Returns the numbers divisiors from 1 to 10
        /// </summary>
        public byte[] Divisors { get; private set; }

        /// <summary>
        /// Returns the number represented in Hexadecimal form
        /// </summary>
        public string HexaValue { get; private set; }

        /// <summary>
        /// Returns the number represented in Octal form
        /// </summary>
        public string OctalValue { get; private set; }

        /// <summary>
        /// Returns the NumberInfo as a String
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("IsPositive:\t\t{0}\r\n", IsPositive);
            sb.AppendFormat("IsNegative:\t\t{0}\r\n", IsNegative);
            sb.AppendFormat("IsInteger:\t\t{0}\r\n", IsInteger);
            sb.AppendFormat("IsFloat:\t\t{0}\r\n", IsFloat);
            sb.AppendFormat("IsNaN:\t\t\t{0}\r\n", IsNaN);
            sb.AppendFormat("IsPositiveInfinity:\t{0}\r\n", IsPositiveInfinity);
            sb.AppendFormat("IsNegativeInfinity:\t{0}\r\n", IsNegativeInfinity);

            sb.AppendFormat("Hexa value:\t\t{0}\r\n", HexaValue);
            sb.AppendFormat("Octal value:\t\t{0}\r\n", OctalValue);

            sb.AppendFormat("BitLength:\t\t{0}\r\n", BitLength);
            sb.AppendFormat("Divisors:\t\t");
            sb.Append(ByteArrayToString(Divisors, "{0}, "));
            sb.AppendLine();

            return sb.ToString();
        }

        /// <summary>
        /// Gets various informations about a number and returns a NumberInfo type with the informations
        /// </summary>
        /// <param name="number">A number</param>
        /// <returns>A NumberInfo type with the informations</returns>
        public static NumberInfo GetNumberInfo(double number)
        {
            NumberInfo ret = new NumberInfo();

            ret.IsPositive = number > 0;
            ret.IsNegative = number < 0;
            ret.IsNegativeInfinity = double.IsNegativeInfinity(number);
            ret.IsPositiveInfinity = double.IsPositiveInfinity(number);
            ret.IsNaN = double.IsNaN(number);
            ret.IsInteger = (number - Math.Truncate(number) == 0);
            ret.IsFloat = !ret.IsInteger;

            if (ret.IsFloat) ret.BitLength = 64;
            else ret.BitLength = Convert.ToString((long)Math.Truncate(number), 2).Length;

            List<byte> divs = new List<byte>(10);
            divs.Add(1);
            for (byte i = 2; i <= 10; i++)
            {
                if (number % i == 0) divs.Add(i);
            }
            ret.Divisors = divs.ToArray();

            if (!ret.IsFloat)
            {
                ret.HexaValue = Convert.ToString((long)Math.Truncate(number), 16);
                ret.OctalValue = Convert.ToString((long)Math.Truncate(number), 8);
            }
            else
            {
                byte[] bytes = BitConverter.GetBytes(number);
                ret.HexaValue = ret.ByteArrayToString(bytes, "{0} ", 16);
                ret.OctalValue = ret.ByteArrayToString(bytes, "{0} ", 8);
            }

            return ret;
        }
    }
}
