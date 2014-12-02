using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections;


namespace MCalculator.Classes.Syntax
{
    /// <summary>
    /// Interaction logic for SyntaxHighlightTextBox.xaml
    /// </summary>
    public partial class SyntaxHighlightTextBox : TextBox
    {
        #region Dependency properties
        public static DependencyProperty LineNumberMarginWidthProperty = DependencyProperty.Register("LineNumberMarginWidth", typeof(double), typeof(SyntaxHighlightTextBox),
                                         new FrameworkPropertyMetadata(15.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static DependencyProperty LineNumberWidthOffsetProperty = DependencyProperty.Register("LineNumberWidthOffset", typeof(double), typeof(SyntaxHighlightTextBox),
                                         new FrameworkPropertyMetadata(15.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public static DependencyProperty LineNumberBrushProperty = DependencyProperty.Register("LineNumberBrush", typeof(Brush), typeof(SyntaxHighlightTextBox),
                                       new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), FrameworkPropertyMetadataOptions.AffectsRender, OnLineNumberBrushChanged));

        public static readonly DependencyProperty HighlightBrushProperty =
                                        DependencyProperty.Register("HighlightBrush", typeof(Brush), typeof(SyntaxHighlightTextBox), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Yellow), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty BackgroundBrushProperty =
                                        DependencyProperty.Register("BackgroundBrush", typeof(Brush), typeof(SyntaxHighlightTextBox), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.White), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty ForegroundBrushProperty =
                                        DependencyProperty.Register("ForegroundBrush", typeof(Brush), typeof(SyntaxHighlightTextBox), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black), FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty SyntaxRulesProperty =
                                        DependencyProperty.Register("SyntaxRules", typeof(SyntaxRuleCollection), typeof(SyntaxHighlightTextBox), new FrameworkPropertyMetadata(new SyntaxRuleCollection(), OnSyntaxRulesChanged));

        public static readonly DependencyProperty CursorColorProperty =
                                        DependencyProperty.Register("CursorColor", typeof(Color), typeof(SyntaxHighlightTextBox), new FrameworkPropertyMetadata(Colors.Black, OnCursorColorChanged));


        #endregion

        #region Routed Events

        public static readonly RoutedEvent SyntaxRulesChangedEvent = EventManager.RegisterRoutedEvent("SyntaxRulesChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(SyntaxHighlightTextBox));

        #endregion

        #region Private members

        private Dictionary<CodeTokenType, SyntaxRuleItem> _rules = null;

        //private Canvas _suggestionCanvas = null;
        /*private ListBox _suggestionList = null;*/
        private double _left_text_border = Double.PositiveInfinity;
        private SyntaxLexer _parser;

        #endregion

        #region Construction

        /// <summary>
        /// Constructor
        /// </summary>
        public SyntaxHighlightTextBox()
        {

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                try
                {
                    // set transparent color 
                    Control.ForegroundProperty.OverrideMetadata(typeof(SyntaxHighlightTextBox), new FrameworkPropertyMetadata(Brushes.Transparent, OnForegroundChanged));

                    Control.BackgroundProperty.OverrideMetadata(typeof(SyntaxHighlightTextBox), new FrameworkPropertyMetadata(OnBackgroundChanged));
                }
                catch (Exception) { }
            }

            this.Loaded += new RoutedEventHandler(SyntaxHighligtTextBox_Loaded);
            this.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            this.AcceptsTab = true;
            this.AcceptsReturn = true;

            HighlightText = new HighlightWordsCollection();
            HighlightText.ListChanged += new EventHandler(HighlightText_ListChanged);
            TabSize = 4;

            InitializeComponent();

        }



        #endregion

        #region Events

        /// <summary>
        /// Notify change of syntax rules
        /// </summary>
        public event RoutedEventHandler SyntaxRulesChanged
        {
            add
            {
                AddHandler(SyntaxRulesChangedEvent, value);
            }

            remove
            {
                RemoveHandler(SyntaxRulesChangedEvent, value);
            }
        }

        #endregion

        #region Public properties


        /// <summary>
        /// Color of cursor 
        /// </summary>
        public Color CursorColor
        {
            get { return (Color)GetValue(CursorColorProperty); }
            set { SetValue(CursorColorProperty, value); }
        }

        /// <summary>
        /// Brush of line numbering 
        /// </summary>
        public Brush LineNumberBrush
        {
            get { return (Brush)GetValue(LineNumberBrushProperty); }
            set { SetValue(LineNumberBrushProperty, value); }
        }

        /// <summary>
        /// Size between text and line number
        /// </summary>
        public double LineNumberWidthOffset
        {
            get { return (Double)GetValue(LineNumberWidthOffsetProperty); }
            set { SetValue(LineNumberWidthOffsetProperty, value); }
        }

        /// <summary>
        /// Line number margin
        /// </summary>
        public double LineNumberMarginWidth
        {
            get { return (Double)GetValue(LineNumberMarginWidthProperty); }
            set { SetValue(LineNumberMarginWidthProperty, value); }
        }


        /// <summary>
        /// Syntax rules definition
        /// </summary>
        public SyntaxRuleCollection SyntaxRules
        {
            get { return (SyntaxRuleCollection)GetValue(SyntaxRulesProperty); }
            set { SetValue(SyntaxRulesProperty, value); }
        }

        /// <summary>
        /// Default background brush
        /// </summary>
        public Brush BackgroundBrush
        {
            get { return (Brush)GetValue(BackgroundBrushProperty); }
            set { SetValue(BackgroundBrushProperty, value); }
        }

        /// <summary>
        /// Default text brush
        /// </summary>
        public Brush ForegroundBrush
        {
            get { return (Brush)GetValue(ForegroundBrushProperty); }
            set { SetValue(ForegroundBrushProperty, value); }
        }

        /// <summary>
        /// Default brush for highlight
        /// </summary>
        public Brush HighlightBrush
        {
            get { return (Brush)GetValue(HighlightBrushProperty); }
            set { SetValue(HighlightBrushProperty, value); }
        }

        /// <summary>
        /// Text for hightlighting
        /// </summary>
        public HighlightWordsCollection HighlightText
        {
            get;
            set;
        }

        /// <summary>
        /// Tab size (spaces)
        /// </summary>
        public int TabSize
        {
            get;
            set;
        }

        /// <summary>
        /// Syntax provider
        /// </summary>
        public SyntaxLexer SyntaxLexer
        {
            get
            {
                return _parser;
            }
            set
            {
                _parser = value;
                if (_parser != null)
                {
                    _parser.Parse(this.Text, this.CaretIndex);
                }
                InvalidateVisual();
            }
        }

        #endregion

        #region Private properties

        /// <summary>
        /// Caching line number format
        /// </summary>
        private FormattedText LastLineNumberFormat
        {
            get;
            set;
        }

        private string Tab
        {
            get
            {
                return new String(' ', TabSize);
            }
        }
        #endregion

        #region Protected overrides

        /// <summary>
        /// Check text changed
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            this.Text = this.Text.Replace("\t", this.Tab);
            if (SyntaxLexer != null)
                SyntaxLexer.Parse(this.Text, CaretIndex);
            this.InvalidateVisual();
        }


        protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {

            // hide intellisense list
            /*if (e.Key == System.Windows.Input.Key.Escape)
            {
                HideSuggestionList();
                e.Handled = true;
            }*/

            // show intellisense list
            //(Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)

            // shift + tab 
            if (e.Key == System.Windows.Input.Key.Tab && e.KeyboardDevice.Modifiers == System.Windows.Input.ModifierKeys.Shift)
            {
                // with selected text
                if (this.SelectedText != String.Empty)
                {
                    string[] lines = this.SelectedText.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (lines[i].StartsWith(this.Tab))
                        {
                            lines[i] = lines[i].Substring(this.Tab.Length);
                        }
                        else
                            lines[i] = lines[i].TrimStart(' ');
                    }
                    this.SelectedText = String.Join(Environment.NewLine, lines);
                }
                else
                {
                    // current line 
                    int index = this.CaretIndex;
                    int last_line = this.Text.LastIndexOf(Environment.NewLine, index);

                    if (last_line == -1)
                    {
                        if (this.Text.Length > 1) last_line = this.Text.Length - 1;
                        else last_line = 0;
                    }

                    int start_line = this.Text.IndexOf(Environment.NewLine, last_line);

                    if (start_line != -1)
                        start_line += Environment.NewLine.Length;
                    else
                        start_line = 0;

                    // find empty spaces
                    int spaces = 0;
                    for (int i = start_line; i < this.Text.Length - 1; i++)
                    {
                        if (this.Text[i] == ' ')
                            spaces++;
                        else
                            break;
                    }

                    if (spaces > TabSize)
                        spaces = TabSize;

                    this.Text = this.Text.Remove(start_line, spaces);

                    // set position of caret
                    if (index >= start_line + spaces)
                        this.CaretIndex = index - spaces;
                    else if (index >= start_line && index < start_line + spaces)
                        this.CaretIndex = start_line;
                    else
                        this.CaretIndex = index;

                }

                e.Handled = true;
            }

            // tab 
            if (e.Key == System.Windows.Input.Key.Tab && e.KeyboardDevice.Modifiers == System.Windows.Input.ModifierKeys.None)
            {
                if (this.SelectedText == String.Empty)
                {
                    int caretPosition = base.CaretIndex;
                    base.Text = base.Text.Insert(caretPosition, this.Tab);
                    base.CaretIndex = caretPosition + TabSize;
                }
                else
                {
                    if (!this.SelectedText.Contains(Environment.NewLine))
                    {
                        this.SelectedText = this.Tab;
                    }
                    else
                    {
                        this.SelectedText = this.Tab + SelectedText.Replace(Environment.NewLine, Environment.NewLine + this.Tab);
                    }

                }
                e.Handled = true;
            }

            // enter respects indenting
            if (e.Key == System.Windows.Input.Key.Return && e.KeyboardDevice.Modifiers == System.Windows.Input.ModifierKeys.None)
            {
                int index = this.CaretIndex;
                int last_line = this.Text.LastIndexOf(Environment.NewLine, index);
                int spaces = 0;

                if (last_line != -1)
                {
                    string line = this.Text.Substring(last_line, this.Text.Length - last_line);

                    int start_line = line.IndexOf(Environment.NewLine);

                    if (start_line != -1)
                        line = line.Substring(start_line).TrimStart('\r', '\n');

                    foreach (char c in line)
                    {
                        if (c == ' ')
                            spaces++;
                        else
                            break;
                    }
                }
                this.Text = this.Text.Insert(index, Environment.NewLine + new String(' ', spaces));
                this.CaretIndex = index + Environment.NewLine.Length + spaces;

                e.Handled = true;
            }

            base.OnPreviewKeyDown(e);
        }


        // rendering 
        protected override void OnRender(DrawingContext drawingContext)
        {
            // draw background 
            drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, this.ActualWidth, this.ActualHeight)));
            drawingContext.DrawRectangle(this.BackgroundBrush, new Pen(), new Rect(0, 0, this.ActualWidth, this.ActualHeight));
            // draw text
            if (this.Text != String.Empty)
            {
                FormattedText ft = new FormattedText(
                this.Text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(this.FontFamily.Source),
                this.FontSize, ForegroundBrush);

                var left_margin = 4.0 + this.BorderThickness.Left + LineNumberMarginWidth;
                var top_margin = 2.0 + this.BorderThickness.Top;

                // Background highlight
                if (HighlightText != null && HighlightText.Count > 0)
                {
                    foreach (string text in HighlightText)
                    {
                        int txtEnd = this.Text.Length;
                        int index = 0;
                        int lastIndex = this.Text.LastIndexOf(text, StringComparison.OrdinalIgnoreCase);

                        while (index <= lastIndex)
                        {
                            index = this.Text.IndexOf(text, index, StringComparison.OrdinalIgnoreCase);

                            Geometry geom = ft.BuildHighlightGeometry(new Point(left_margin, top_margin - this.VerticalOffset), index, text.Length);
                            if (geom != null)
                            {
                                drawingContext.DrawGeometry(HighlightBrush, null, geom);
                            }
                            index += 1;
                        }
                    }
                }

                if (SyntaxLexer != null)
                {
                    // set color of tokens by syntax rules
                    foreach (var t in SyntaxLexer.Tokens)
                    {
                        SyntaxRuleItem rule = GetSyntaxRule(t.TokenType);
                        if (rule != null) ft.SetForegroundBrush(rule.Foreground, t.Start, t.Length);
                    }
                }
                ft.Trimming = TextTrimming.None;

                // left from first char boundary

                double left_border = GetRectFromCharacterIndex(0).Left;
                if (!Double.IsInfinity(left_border))
                    _left_text_border = left_border;

                drawingContext.DrawText(ft, new Point(_left_text_border - this.HorizontalOffset, top_margin - this.VerticalOffset));


                // draw lines
                if (this.GetLastVisibleLineIndex() != -1)
                {
                    LastLineNumberFormat = GetLineNumbers();
                }
                if (LastLineNumberFormat != null)
                {
                    LastLineNumberFormat.SetForegroundBrush(LineNumberBrush);
                    drawingContext.DrawText(LastLineNumberFormat, new Point(3, top_margin));
                }
            }

        }

        #endregion

        #region Dependency properties callbacks


        /// <summary>
        /// Set new basic foreground after change cursor's color
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnCursorColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SyntaxHighlightTextBox sh = (SyntaxHighlightTextBox)d;
            Color c = (Color)e.NewValue;
            sh.Background = new SolidColorBrush(sh.GetAlphaColor(c));
        }

        /// <summary>
        /// invalidate after change of line number's brush
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnLineNumberBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SyntaxHighlightTextBox)d).InvalidateVisual();
        }


        /// <summary>
        /// always set transparent color
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            if (e.NewValue != Brushes.Transparent)
            {
                ((SyntaxHighlightTextBox)d).Foreground = Brushes.Transparent;
            }
        }



        /// <summary>
        ///  always set transparent color
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SyntaxHighlightTextBox sh = (SyntaxHighlightTextBox)d;

            SolidColorBrush b = new SolidColorBrush(sh.GetAlphaColor(sh.CursorColor));
            SolidColorBrush a = e.NewValue as SolidColorBrush;
            if (a == null || a.Color != b.Color)
            {
                sh.Background = b;
            }

        }

        /// <summary>
        /// Syntax rules changed
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param> 
        static void OnSyntaxRulesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            ((SyntaxHighlightTextBox)obj)._rules = null; // flush cache

            // do event
            RoutedEventArgs arg = new RoutedEventArgs(SyntaxHighlightTextBox.SyntaxRulesChangedEvent);
            ((SyntaxHighlightTextBox)obj).RaiseEvent(arg);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Invalidate after collection changed
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void HighlightText_ListChanged(object source, EventArgs e)
        {
            this.InvalidateVisual();
        }

        /// <summary>
        /// Return syntax rule for category
        /// </summary>
        /// <param name="tc"></param>
        /// <returns></returns>
        private SyntaxRuleItem GetSyntaxRule(CodeTokenType tc)
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
                return null;

            // caching in dictionary
            if (_rules == null)
            {
                _rules = new Dictionary<CodeTokenType, SyntaxRuleItem>();
                foreach (SyntaxRuleItem i in SyntaxRules)
                {
                    _rules[i.RuleType] = i;
                }
            }

            if (_rules.ContainsKey(tc))
                return _rules[tc];

            return null;
        }

        /// <summary>
        /// Return transparent color (for right cursor color)
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private Color GetAlphaColor(Color c)
        {
            return Color.FromArgb(0, (byte)(255 - c.R), (byte)(255 - c.G), (byte)(255 - c.B));
        }

        /// <summary>
        /// Set schrollChanged event for scrollbars
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SyntaxHighligtTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (VisualTreeHelper.GetChildrenCount(this) > 0)
            {
                var bd = VisualTreeHelper.GetChild(this, 0); // Border
                var gd = VisualTreeHelper.GetChild(bd, 0); // Grid
                var sv = VisualTreeHelper.GetChild(gd, 0) as ScrollViewer; // Scrollbar
                sv.ScrollChanged += (s2, e2) => { this.InvalidateVisual(); };

               // _suggestionCanvas = VisualTreeHelper.GetChild(gd, 2) as Canvas; // canvas
               // _suggestionList = VisualTreeHelper.GetChild(_suggestionCanvas, 0) as ListBox; // listbox
                
                /*HideSuggestionList();*/

                // refresh 
                InvalidateVisual();
            }
        }

        /*/// <summary>
        /// Show suggestion list
        /// </summary>
        private void ShowSuggestionList()
        {
            Point position = GetRectFromCharacterIndex(this.CaretIndex).BottomRight;

            _suggestionCanvas.IsHitTestVisible = true;

            double left = position.X - LineNumberWidthOffset - LineNumberMarginWidth;
            double top = position.Y;

            if (left + _suggestionList.ActualWidth > _suggestionCanvas.ActualWidth)
                left = _suggestionCanvas.ActualWidth - _suggestionList.ActualWidth;

            if (top + _suggestionList.ActualHeight > _suggestionCanvas.ActualHeight)
                top = _suggestionCanvas.ActualHeight - _suggestionList.ActualHeight;

            Canvas.SetLeft(_suggestionList, left);
            Canvas.SetTop(_suggestionList, top);
            _suggestionList.Visibility = Visibility.Visible;
            _suggestionList.Focus();
        }*/

        public Point GetCurrentXY()
        {
            Point ret = new Point();
            Point position = GetRectFromCharacterIndex(this.CaretIndex).BottomRight;
            ret.X = position.X - LineNumberWidthOffset - LineNumberMarginWidth;
            ret.Y = position.Y;
            return ret;
        }

        /*/// <summary>
        /// Hide suggestion list
        /// </summary>
        private void HideSuggestionList()
        {
            _suggestionCanvas.IsHitTestVisible = false;
            _suggestionList.Visibility = Visibility.Hidden;
        }*/


        /// <summary>
        /// Returns the line widths for use with the wrap with overflow.
        /// </summary>
        /// <returns></returns>
        private Double[] VisibleLineWidthsIncludingTrailingWhitespace()
        {

            int firstLine = this.GetFirstVisibleLineIndex();
            int lastLine = Math.Max(this.GetLastVisibleLineIndex(), firstLine);
            Double[] lineWidths = new Double[lastLine - firstLine + 1];
            if (lineWidths.Length == 1)
            {
                lineWidths[0] = MeasureString(this.Text);
            }
            else
            {
                for (int i = firstLine; i <= lastLine; i++)
                {
                    string lineString = this.GetLineText(i);
                    lineWidths[i - firstLine] = MeasureString(lineString);
                }
            }
            return lineWidths;
        }

        /// <summary>
        /// Returns the width of the string in the font and fontsize of the textbox including the trailing white space.
        /// Used for wrap with overflow.
        /// </summary>
        /// <param name="str">The string to measure</param>
        /// <returns></returns>
        private double MeasureString(string str)
        {
            FormattedText formattedText = new FormattedText(
                 str,
                   System.Globalization.CultureInfo.CurrentCulture,
                   FlowDirection.LeftToRight,
                   new Typeface(this.FontFamily.Source),
                   this.FontSize,
                  new SolidColorBrush(Colors.Black));
            if (str == "")
            {
                return formattedText.WidthIncludingTrailingWhitespace;
            }
            else if (str.Substring(0, 1) == "\t")
            {
                return formattedText.WidthIncludingTrailingWhitespace;
            }
            else
            {
                return formattedText.WidthIncludingTrailingWhitespace;
            }
        }

        /// <summary>
        /// Generates FormattedText for line numbers
        /// </summary>
        /// <returns></returns>
        private FormattedText GetLineNumbers()
        {
            int firstLine = GetFirstVisibleLineIndex();
            int lastLine = GetLastVisibleLineIndex();
            StringBuilder sb = new StringBuilder();
            double max_size = 0;

            for (int i = firstLine; i <= lastLine; i++)
            {
                string num = (i + 1) + "\n";

                double size = MeasureString(num);
                if (size > max_size)
                    max_size = size;

                sb.Append(num);
            }
            // set new max size
            LineNumberMarginWidth = max_size + LineNumberWidthOffset;

            string lineNumberString = sb.ToString();
            FormattedText lineNumbers = new FormattedText(
                  lineNumberString,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(this.FontFamily.Source),
                    this.FontSize,
                    LineNumberBrush);
            return lineNumbers;
        }

        #endregion

    }
}
