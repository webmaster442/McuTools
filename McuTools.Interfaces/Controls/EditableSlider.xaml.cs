using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace McuTools.Interfaces.Controls
{
    public partial class EditableSlider : UserControl
    {

        public static DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(EditableSlider), new PropertyMetadata(0.0d));
        public static DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(EditableSlider), new PropertyMetadata(10.0d));
        public static DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(EditableSlider), new PropertyMetadata(3.0d));
        public static DependencyProperty DefaultValueProperty = DependencyProperty.Register("DefaultValue", typeof(double), typeof(EditableSlider), new PropertyMetadata(3.0d));

        public event RoutedEventHandler ValueChanged;

        private bool _editOnClickSetting = false;

        /// <summary>
        /// True if dragging via mouse, false otherwise
        /// </summary>
        private bool _isDragging = false;

        /// <summary>
        /// True if the textbox is enabled and being edited, false otherwise
        /// </summary>
        private bool _isInEditMode = false;

        /// <summary>
        /// True if the user started dragging, false otherwise
        /// </summary>
        private bool _hasBeenDragging = false;

        private string _displayFormat = "0.00";

        private bool _usingCustomDisplayFormat = false;

        bool _isInitialized = false;

        private bool _isOverEditValueButton = false;

        public EditableSlider()
        {
            InitializeComponent();
            _isInitialized = true;
            this.SizeChanged += new SizeChangedEventHandler(EditableSlider_SizeChanged);
        }

        void EditableSlider_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!_isInitialized) return;
            UpdateView();
        }

        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set
            {
                SetValue(MinimumProperty, value);
                UpdateView();
            }
        }

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set
            {
                SetValue(MaximumProperty, value);
                UpdateView();
            }
        }

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set
            {
                SetValue(ValueProperty, value);
                //_value = value;
                UpdateView();
                UpdateText();
                if (ValueChanged != null) ValueChanged(this, new RoutedEventArgs());
            }
        }

        public double DefaultValue
        {
            get { return (double)GetValue(DefaultValueProperty); }
            set { SetValue(DefaultValueProperty, value); }
        }

        public string DisplayFormat
        {
            get { return _displayFormat; }
            set
            {
                _usingCustomDisplayFormat = true;
                if (_displayFormat == value) return;
                _displayFormat = value;
                UpdateText();
            }
        }

        private void rectBase_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            rectBase.CaptureMouse();
            _isDragging = true;
            _hasBeenDragging = false;
        }

        private void rectBase_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            rectBase.ReleaseMouseCapture();
            ReleaseMouseCapture();
            if (_editOnClickSetting)
            {
                _isDragging = false;
                if (!_hasBeenDragging) EnterEditMode();
            }
            else
            {
                if (_isDragging)
                {
                    double x = e.GetPosition(this.rectBase).X;
                    if (x < 0) x = CorrectGetPosition(rectBase).X;
                    UpdatePercent(x);
                }
            }
            _isDragging = false;
        }
        public Point CorrectGetPosition(Visual relativeTo)
        {
            return relativeTo.PointFromScreen(new Point(System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y));
        }

        private void rectBase_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                double x = e.GetPosition(this.rectBase).X;
                if (x < 0) x = CorrectGetPosition(rectBase).X;
                UpdatePercent(x);
                _hasBeenDragging = true;
            }
        }

        private void UpdatePercent(double xPos)
        {
            if (xPos < 0) xPos = 0;
            if (xPos > rectBase.ActualWidth) xPos = rectBase.ActualWidth;
            rectPercent.Width = xPos;
            double valuePercent = xPos / rectBase.ActualWidth;
            Value = (Maximum - Minimum) * valuePercent + Minimum;
        }

        private void ChangeDisplayFormatIfNeeded(string newFormat)
        {
            if (_usingCustomDisplayFormat) return;
            if (_displayFormat == newFormat) return;
            _displayFormat = newFormat;
            UpdateText();
        }

        private void UpdateView()
        {
            double valuePercent = (Value - Minimum) / (Maximum - Minimum);
            rectPercent.Width = rectBase.ActualWidth * valuePercent;

            double range = Math.Abs(Maximum - Minimum);
            if (range >= 40) ChangeDisplayFormatIfNeeded("0");
            else if (range >= 10) ChangeDisplayFormatIfNeeded("0.0");
            else if (range >= 0.5) ChangeDisplayFormatIfNeeded("0.00");
            else ChangeDisplayFormatIfNeeded("0.0000");
        }

        private void UpdateText()
        {
            textValue.Text = Value.ToString(_displayFormat);
            textValueEdit.Text = textValue.Text;
        }

        private void EnterEditMode()
        {
            _isInEditMode = true;
            textValueEdit.Visibility = Visibility.Visible;
            textValue.Visibility = Visibility.Collapsed;
            textValueEdit.Focus();
            textValueEdit.SelectAll();
        }

        public void ExitEditMode()
        {
            _isInEditMode = false;
            textValueEdit.Visibility = Visibility.Collapsed;
            textValue.Visibility = Visibility.Visible;
            double result;
            if (double.TryParse(textValueEdit.Text, out result))
            {
                if (result < Minimum) result = Minimum;
                if (result > Maximum) result = Maximum;
                Value = result;
            }
            else UpdateText();
        }

        private void textValueEdit_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!_isOverEditValueButton) ExitEditMode();
        }

        private void textValueEdit_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) ExitEditMode();
        }

        private void btnEditValue_Click(object sender, RoutedEventArgs e)
        {
            if (_isInEditMode) ExitEditMode();
            else EnterEditMode();
        }

        private void btnEditValue_MouseEnter(object sender, MouseEventArgs e)
        {
            _isOverEditValueButton = true;
        }

        private void btnEditValue_MouseLeave(object sender, MouseEventArgs e)
        {
            _isOverEditValueButton = false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            double value = Value;
            Value = value;
        }

        private void btnResetValue_Click(object sender, RoutedEventArgs e)
        {
            Value = DefaultValue;
        }

        private void rootGrid_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool value = Convert.ToBoolean(e.NewValue);
            if (!value) VisualStateManager.GoToState(this, "Disabled", false);
            else VisualStateManager.GoToState(this, "Enabled", false);
        }
    }
}
