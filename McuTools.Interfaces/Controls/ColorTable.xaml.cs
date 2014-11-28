using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace McuTools.Interfaces.Controls
{
    /// <summary>
    /// Interaction logic for ColorTable.xaml
    /// </summary>
    public partial class ColorTable : UserControl
    {
        public ColorTable()
        {
            InitializeComponent();
        }

        public static DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorTable));

        public event RoutedEventHandler SelectedColorChanged;

        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        private void ColorClicked(object sender, RoutedEventArgs e)
        {
            Rectangle s = (Rectangle)sender;
            if (s.Fill is SolidColorBrush)
            {
                SolidColorBrush fill = (SolidColorBrush)s.Fill;
                this.SelectedColor = fill.Color;
                if (SelectedColorChanged != null) SelectedColorChanged(this, new RoutedEventArgs());
            }
        }
    }
}
