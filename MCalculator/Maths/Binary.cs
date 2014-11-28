using System;

namespace MCalculator.Maths
{
    /// <summary>
    /// Binary Functions
    /// </summary>
    public static class Binary
    {
        private enum Operation
        {
            NOT, AND, OR, XOR, NAND, NOR, EQ
        }

        private static double Process(double param1, double param2, Operation op)
        {
            dynamic p1, p2;
            if (param1 > 0 && param2 > 0)
            {
                p1 = Convert.ToUInt64(Math.Truncate(param1));
                p2 = Convert.ToUInt64(Math.Truncate(param2));
            }
            else
            {
                p1 = Convert.ToInt64(Math.Truncate(param1));
                p2 = Convert.ToInt64(Math.Truncate(param2));
            }

            switch (op)
            {
                case Operation.NOT:
                    return Convert.ToDouble(~p1);
                case Operation.AND:
                    return Convert.ToDouble(p1 & p2);
                case Operation.OR:
                    return Convert.ToDouble(p1 | p2);
                case Operation.XOR:
                    return Convert.ToDouble(p1 ^ p2);
                case Operation.EQ:
                    return Convert.ToDouble(~(p1 ^ p2));
                case Operation.NAND:
                    return Convert.ToDouble(~(p1 & p2));
                case Operation.NOR:
                    return Convert.ToDouble(~(p1 | p2));
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Bitwise NOT operation on the numbers bits. Number is represented as 64 bit integer
        /// </summary>
        /// <param name="num">input number</param>
        public static double NOT(double num)
        {
            return Process(num, 1, Operation.NOT);
        }

        /// <summary>
        /// Bitwise AND operation on two numbers bits. Numbers are represented as 64 bit integers
        /// </summary>
        /// <param name="p1">Input Number #1</param>
        /// <param name="p2">Input Number #2</param>
        public static double AND(double p1, double p2)
        {
            return Process(p1, p2, Operation.AND);
        }

        /// <summary>
        /// Bitwise OR operation on two numbers bits. Numbers are represented as 64 bit integers
        /// </summary>
        /// <param name="p1">Input Number #1</param>
        /// <param name="p2">Input Number #2</param>        
        public static double OR(double p1, double p2)
        {
            return Process(p1, p2, Operation.OR);
        }
        /// <summary>
        /// Bitwise XOR operation on two numbers bits. Numbers are represented as 64 bit integers
        /// </summary>
        /// <param name="p1">Input Number #1</param>
        /// <param name="p2">Input Number #2</param>        
        public static double XOR(double p1, double p2)
        {
            return Process(p1, p2, Operation.XOR);
        }

        /// <summary>
        /// Bitwise Equality/XNOR operation on two numbers bits. Numbers are represented as 64 bit integers
        /// </summary>
        /// <param name="p1">Input Number #1</param>
        /// <param name="p2">Input Number #2</param>        
        public static double EQ(double p1, double p2)
        {
            return Process(p1, p2, Operation.EQ);
        }

        /// <summary>
        /// Bitwise NAND operation on two numbers bits. Numbers are represented as 64 bit integers
        /// </summary>
        /// <param name="p1">Input Number #1</param>
        /// <param name="p2">Input Number #2</param>        
        public static double NAND(double p1, double p2)
        {
            return Process(p1, p2, Operation.NAND);
        }

        /// <summary>
        /// Bitwise NOR operation on two numbers bits. Numbers are represented as 64 bit integers
        /// </summary>
        /// <param name="p1">Input Number #1</param>
        /// <param name="p2">Input Number #2</param>        
        public static double NOR(double p1, double p2)
        {
            return Process(p1, p2, Operation.NOR);
        }

        /// <summary>
        /// Converts a number to decimal from a given number system
        /// </summary>
        /// <param name="number">The input number as string</param>
        /// <param name="system">source system. Can be: 2, 8 or 16</param>
        public static double FromSystem(string number, int system)
        {
            return Convert.ToInt64(number, system);
        }

        /// <summary>
        /// Converts a decimal number to a given number sytem
        /// </summary>
        /// <param name="number">the inpu decimal number</param>
        /// <param name="system">output system. Can be: 2, 8 or 16</param>
        public static string ToSystem(double number, int system)
        {
            return Convert.ToString((long)number, system);
        }
    }
}
