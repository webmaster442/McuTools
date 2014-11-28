using McuTools.Interfaces.WPF;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace McuTools.Interfaces.Controls
{
    /// <summary>
    /// Interaction logic for ShaderTabControl.xaml
    /// </summary>
    public partial class ShaderTabControl : UserControl
    {

        private bool _selectionchanged;
        private UserControl _popupchild;
        private Style _roundcorner;
        private Storyboard[] _openanims, _closeanims;
        private Random _randomgenerator;

        public ShaderTabControl()
        {
            InitializeComponent();
            _selectionchanged = true;
            _randomgenerator = new Random();
            _roundcorner = (Style)(this.FindResource("RoundCorners"));
            _openanims = new Storyboard[]
            {
                WpfHelpers.FindStoryBoard(this, "PopupSlideLeftIn"),
                WpfHelpers.FindStoryBoard(this, "PopupSlideRightIn"),
                WpfHelpers.FindStoryBoard(this, "PopupSlideTopIn"),
                WpfHelpers.FindStoryBoard(this, "PopupSlideBottomIn")
            };
            _closeanims = new Storyboard[]
            {
                WpfHelpers.FindStoryBoard(this, "PopupSlideLeftOut"),
                WpfHelpers.FindStoryBoard(this, "PopupSlideRightOut"),
                WpfHelpers.FindStoryBoard(this, "PopupSlideTopOut"),
                WpfHelpers.FindStoryBoard(this, "PopupSlideBottomOut")
            };
            _closeanims[0].Completed += PopupClose_Completed;
            _closeanims[1].Completed += PopupClose_Completed;
            _closeanims[2].Completed += PopupClose_Completed;
            _closeanims[3].Completed += PopupClose_Completed;
        }

        public event RoutedEventHandler PopUpClosed;

        #region menu
        private void CbItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Tabs.Items.Count < 1) return;
            if (_selectionchanged) SwitchTo(CbItems.SelectedIndex);
        }

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            if (Tabs.Items.Count < 2) return;
            SwitchTo(--Tabs.SelectedIndex);
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (Tabs.Items.Count < 2) return;
            SwitchTo(++Tabs.SelectedIndex);
        }

        private void DestroyObject(IDisposable objectref)
        {
            if (objectref != null)
            {
                objectref.Dispose();
                objectref = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        private void PopOut_Click(object sender, RoutedEventArgs e)
        {
            UIElement control = null;
            UIElement clone = null;
            ShaderTabPopoutWin popout;
            bool disposeable;

            if (Tabs.Items.Count < 1) return;
            control = GetCurrentControl();
            disposeable = control is IDisposable;

            if (control is IFixedTool)
            {
                WpfHelpers.ExceptionDialog("The Current tool doesn't support this function");
                return;
            }

            if (disposeable)
            {
                MessageBoxResult r = MessageBox.Show("Warning. Poping out this tool to a new window will reset it's workflow.\r\nDo you want to continue?", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (r == MessageBoxResult.No) return;
                clone = (UIElement)Activator.CreateInstance(control.GetType());
                DestroyObject(control as IDisposable);
            }

            popout = new ShaderTabPopoutWin();
            popout.Width = this.ActualWidth;
            popout.Height = this.ActualHeight;
            popout.GlassTitle = (Tabs.Items[Tabs.SelectedIndex] as TabItem).Header.ToString();

            RemoveCurrenctontrol();
            if (disposeable) popout.TabContent = clone;
            else popout.TabContent = control;
            popout.Show();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            RemoveCurrenctontrol();
        }
        #endregion

        #region Popup Window
        private void RunRandomAnim(Storyboard[] animations)
        {
            int index = _randomgenerator.Next(0, animations.Length);
            animations[index].Begin();
        }

        private void ResetAnimation()
        {
            WpfHelpers.FindStoryBoard(this, "Reset").Begin();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            RunRandomAnim(_closeanims);
        }

        public void ClosePopup()
        {
            BtnClose_Click(this, null);
        }

        public void OpenPopup(UserControl content, string Header)
        {
            try { PopupWin.Children.Add(content); }
            catch (ArgumentException) { return; }
            //content.Margin = new Thickness(2, 0, 2, 2);
            _popupchild = content;
            WindowText.Text = Header;
            content.Margin = new Thickness(10, 0, 10, 10);
            Grid.SetRow(content, 1);
            Grid.SetColumnSpan(content, 2);
            ResetAnimation();
            RunRandomAnim(_openanims);
            Tabs.IsEnabled = false;
            Controls.IsEnabled = false;
        }

        private void PopupClose_Completed(object sender, EventArgs e)
        {
            if (_popupchild is IDisposable) (_popupchild as IDisposable).Dispose();
            PopupWin.Children.Remove(_popupchild);
            _popupchild = null;
            Tabs.IsEnabled = true;
            Controls.IsEnabled = true;
            if (PopUpClosed != null) PopUpClosed(this, new RoutedEventArgs());
        }
        #endregion

        #region Public API
        public UIElement GetCurrentControl()
        {
            if (Tabs.SelectedIndex < 0) return null;
            TabItem current = (TabItem)Tabs.Items[Tabs.SelectedIndex];
            return (UIElement)current.Content;
        }

        public void RemoveCurrenctontrol()
        {
            if (Tabs.SelectedIndex < 0) return;
            TabItem current = (TabItem)Tabs.Items[Tabs.SelectedIndex];
            if (current.Content is IDisposable)
            {
                (current.Content as IDisposable).Dispose();
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            CbItems.Items.RemoveAt(Tabs.SelectedIndex);
            Tabs.Items.Remove(current);
        }

        public void SwitchTo(int index)
        {
            if (index < 0) index = Tabs.Items.Count - 1;
            if (index > Tabs.Items.Count - 1) index = 0;
            Tabs.SelectedIndex = index;
            CbItems.SelectedIndex = index;
        }

        public void AddControl(Control Control, string HeaderText)
        {
            TabItem ti = new TabItem();
            ti.Style = _roundcorner;
            ti.Header = HeaderText;
            ti.Content = Control;
            int index = Tabs.Items.Add(ti);

            ComboBoxItem ci = new ComboBoxItem();
            ci.Content = HeaderText;
            CbItems.Items.Add(ci);

            SwitchTo(index);
        }

        public void SetCurrentTitle(string s)
        {
            TabItem current = (TabItem)Tabs.Items[Tabs.SelectedIndex];
            current.Header = s;
        }

        public void CloseAllTabs()
        {
            while (Tabs.Items.Count > 0)
            {
                SwitchTo(0);
                RemoveCurrenctontrol();
            }
        }

        public string PopUpHeader
        {
            get { return WindowText.Text; }
        }
        #endregion


    }
}
