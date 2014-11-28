using McuTools.Interfaces.WPF;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MTools.ToolsDigital
{
    /// <summary>
    /// Interaction logic for LEDMatrixEditor.xaml
    /// </summary>
    public partial class LEDMatrixEditor : UserControl
    {
        private bool _loaded;

        public LEDMatrixEditor()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            _loaded = true;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            var items = WpfHelpers.FindChildren<Rectangle>(Grid8x);
            foreach (Rectangle item in items)
            {
                item.Fill = new SolidColorBrush(Colors.Black);
            }
        }

        private void Fill_Click(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            var items = WpfHelpers.FindChildren<Rectangle>(Grid8x);
            foreach (Rectangle item in items)
            {
                item.Fill = new SolidColorBrush(Colors.Red);
            }
        }

        private int[,] Matrix
        {
            get
            {
                Rectangle r;
                int[,] matrix = new int[8, 8];
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        r = Functions.GetRectangle(Grid8x, i, j);
                        if (Functions.isSegmentOn(r)) matrix[i, j] = 1;
                        else matrix[i, j] = 0;
                    }
                }
                return matrix;
            }
            set
            {
                Rectangle r;
                for (int i = 0; i < value.GetLength(0); i++)
                {
                    for (int j = 0; j < value.GetLength(1); j++)
                    {
                        r = Functions.GetRectangle(Grid8x, i, j);
                        if (value[i, j] == 0) r.Fill = new SolidColorBrush(Colors.Black);
                        else r.Fill = new SolidColorBrush(Colors.Red);
                    }
                }
            }
        }

        private int[,] Rotate(int[,] data, int angle)
        {
            int[,] ret = new int[data.GetLength(0), data.GetLength(1)];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (angle / 90 == 1) ret[i, j] = data[7 - j, i];
                    else ret[i, j] = data[j, 7 - i];
                }
            }
            return ret;
        }

        private int[,] Flip(int[,] data, bool horizontal)
        {
            int[,] ret = new int[data.GetLength(0), data.GetLength(1)];

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (horizontal) ret[i, j] = data[i, 7 - j];
                    else ret[i, j] = data[7 - i, j];
                }
            }
            return ret;
        }

        private int[,] Invert(int[,] data)
        {
            int[,] ret = new int[data.GetLength(0), data.GetLength(1)];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (data[i, j] == 0) ret[i, j] = 1;
                    else ret[i, j] = 0;
                }
            }
            return ret;
        }

        private int[,] Shift(int[,] data, int direction)
        {
            int[,] ret = new int[data.GetLength(0), data.GetLength(1)];
            int i, j;
            switch (direction)
            {
                case 0: //up
                    for (i = 0; i < 7; i++)
                    {
                        for (j = 0; j < 8; j++) ret[i, j] = data[i + 1, j];
                    }
                    break;
                case 1: //down
                    for (i = 1; i < 8; i++)
                    {
                        for (j = 0; j < 8; j++) ret[i, j] = data[i - 1, j];
                    }
                    break;
                case 2: //right
                    for (i = 0; i < 8; i++)
                    {
                        for (j = 0; j < 7; j++) ret[i, j] = data[i, j + 1];
                    }
                    break;
                case 3: //left
                    for (i = 0; i < 8; i++)
                    {
                        for (j = 1; j < 8; j++) ret[i, j] = data[i, j - 1];
                    }
                    break;
            }
            return ret;
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle segment = (Rectangle)sender;
            SolidColorBrush segmentfill = (SolidColorBrush)segment.Fill;
            if (segmentfill.Color == Colors.Black) segment.Fill = new SolidColorBrush(Colors.Red);
            else segment.Fill = new SolidColorBrush(Colors.Black);
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            int[,] m = Matrix;

            if (DisRot90.IsChecked == true) m = Rotate(Matrix, 90);
            else if (DisRotM90.IsChecked == true) m = Rotate(Matrix, 90);
            else if (DisRot180.IsChecked == true) m = Flip(Matrix, true);

            int[] rows = new int[8];
            //Rectangle r;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (m[i, j] == 1) rows[i] += 1 << (7 - j);
                }
            }

            StringBuilder sb = new StringBuilder("const char matrix[] = {");
            for (int i = 0; i < rows.Length; i++)
            {
                sb.Append(rows[i].ToString());
                if (i != rows.Length - 1) sb.Append(", ");
            }
            sb.Append("};\r\n");
            Tabs.SelectedIndex = 1;
            TbOutput.Text += sb.ToString();
        }

        private void ClearGen_Click(object sender, RoutedEventArgs e)
        {
            TbOutput.Text = "";
        }

        private void FlipX_Click(object sender, RoutedEventArgs e)
        {
            int[,] data = Flip(Matrix, false);
            Matrix = data;
        }

        private void FlipY_Click(object sender, RoutedEventArgs e)
        {
            int[,] data = Flip(Matrix, true);
            Matrix = data;
        }

        private void Rotate90cw_Click(object sender, RoutedEventArgs e)
        {
            int[,] data = Rotate(Matrix, +90);
            Matrix = data;
        }

        private void Rotate90ccw_Click(object sender, RoutedEventArgs e)
        {
            int[,] data = Rotate(Matrix, -90);
            Matrix = data;
        }

        private void Invert_Click(object sender, RoutedEventArgs e)
        {
            int[,] data = Invert(Matrix);
            Matrix = data;
        }

        private void ShiftUP_Click(object sender, RoutedEventArgs e)
        {
            int[,] data = Shift(Matrix, 0);
            Matrix = data;
        }

        private void ShiftLeft_Click(object sender, RoutedEventArgs e)
        {
            int[,] data = Shift(Matrix, 2);
            Matrix = data;
        }

        private void ShiftRight_Click(object sender, RoutedEventArgs e)
        {
            int[,] data = Shift(Matrix, 3);
            Matrix = data;
        }

        private void ShiftDown_Click(object sender, RoutedEventArgs e)
        {
            int[,] data = Shift(Matrix, 1);
            Matrix = data;
        }
    }
}
