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
}
