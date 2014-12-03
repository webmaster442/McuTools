using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace McuTools.Interfaces.WPF
{
    /// <summary>
    /// Converts a double number to string, with 4 digits precision
    /// </summary>
    [ValueConversion(typeof(double), typeof(string))]
    public class DoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double round = Math.Round((double)(value), 4);
            return round.ToString(culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(value);
        }
    }

    /// <summary>
    /// Converts boolean data to visibility information
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class VisibilityConv : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value) return Visibility.Visible;
            else return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility v = (Visibility)value;
            if (v == Visibility.Collapsed) return false;
            else return true;
        }
    }

    /// <summary>
    /// Converts a bool value to negated bool value
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class NegateConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }

    /// <summary>
    /// Converts a string to trimmed output text
    /// </summary>
    [ValueConversion(typeof(string), typeof(string))]
    public class TrimConverter : IValueConverter
    { 
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int len = 60;
            if (parameter != null) len = System.Convert.ToInt32(parameter); 
            string input = (string)value;
            if (input == null) return null;
            if (input.Length < len) return input;
            else return input.Substring(0, len) + "...";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }


    /// <summary>
    /// Converts string to line numbers
    /// </summary>
    [ValueConversion(typeof(string), typeof(int))]
    public class LineNumberConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = (string)value;
            int lines = 0;
            for (int i=0; i<input.Length; i++)
            {
                if (input[i] == '\n') ++lines;
            }
            return lines;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts string to word numbers
    /// </summary>
    [ValueConversion(typeof(string), typeof(int))]
    public class WordNumberConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = ((string)value).Trim();
            var q = input.Split(' ').Length;
            return q;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts int to file size
    /// </summary>
    [ValueConversion(typeof(int), typeof(string))]
    public class FileSizeConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int size = (int)value;
            string postfix = "";
            float outp = 0.0f;
            if (size > 1073741824)
            {
                outp = size / 1073741824.0f;
                postfix = "GiB";
            }
            else if (size > 1048576)
            {
                outp = size / 1048576.0f;
                postfix = "MiB";
            }
            else if (size > 1024)
            {
                outp = size / 1024.0f;
                postfix = "KiB";
            }
            else
            {
                outp = size;
                postfix = "Bytes";
            }
            return string.Format("{0:0.000} {1}", outp, postfix);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
