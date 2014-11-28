using System.Windows;
using System.Windows.Controls;

namespace MCalculator.Classes
{
    internal class CompleteButton: Button
    {
        public static DependencyProperty CompleteTextProperty = DependencyProperty.Register("CompleteText", typeof(string), typeof(CompleteButton));

        public string CompleteText
        {
            get { return (string)GetValue(CompleteTextProperty); }
            set { SetValue(CompleteTextProperty, value); }
        }
    }
}
