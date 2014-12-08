using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace McuShell.Kernel
{
    /// <summary>
    /// Extension methoods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Converts Enum description attributes to user friendly string
        /// </summary>
        /// <typeparam name="T">Enumeration type</typeparam>
        /// <param name="enumerationValue">Enumeration value</param>
        /// <returns>user friendly description string</returns>
        public static string GetDescription<T>(this T enumerationValue) where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            return enumerationValue.ToString();

        }

        /// <summary>
        /// Writes an Enumerable collection's items to the console
        /// </summary>
        /// <typeparam name="T">Type of items in collection</typeparam>
        /// <param name="collection">The collection to write to the console</param>
        public static void WriteToConsole<T>(this IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                Console.WriteLine(item.ToString());
            }
        }
    }
}
