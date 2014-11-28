using McuTools.Interfaces.Controls;
using McuTools.Interfaces.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MCalculator.UserInterface
{
    /// <summary>
    /// Interaction logic for InputForm.xaml
    /// </summary>
    public partial class InputForm : Window
    {
        public InputForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Adds a Range slider to the input form
        /// </summary>
        /// <param name="caption">Control label</param>
        /// <param name="name">Control name</param>
        /// <param name="minimum">Minimum value</param>
        /// <param name="maximum">Maximum value</param>
        /// <param name="value">Default value</param>
        public void AddRangeSlider(string caption, string name, double minimum, double maximum, double value)
        {
            Dispatcher.Invoke(() =>
            {
                TextBlock t = new TextBlock();
                t.Text = caption;
                t.Margin = new Thickness(10);
                ItemsPanel.Children.Add(t);
                EditableSlider slider = new EditableSlider();
                slider.Name = name;
                slider.Minimum = minimum;
                slider.Maximum = maximum;
                slider.Value = value;
                slider.DefaultValue = value;
                slider.Margin = new Thickness(20, 0, 20, 10);
                ItemsPanel.Children.Add(slider);
            });
        }

        /// <summary>
        /// Gets a control value by the control's name
        /// </summary>
        /// <param name="controlname">The name of the control, witch's value will be returned</param>
        public dynamic this[string controlname]
        {
            get
            {
                if (string.IsNullOrEmpty(controlname)) throw new Exception("Can't find control value: " + controlname);
                foreach (var control in WpfHelpers.FindChildren<FrameworkElement>(ItemsPanel))
                {
                    if (control.Name == controlname)
                    {
                        Type t = control.GetType();
                        if (t == typeof(EditableSlider)) return (control as EditableSlider).Value;
                    }
                }
                throw new Exception("Can't find control value: " + controlname);
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
