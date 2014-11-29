using McuTools.Classes;
using McuTools.Interfaces;
using McuTools.Interfaces.Controls;
using McuTools.Interfaces.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media.Imaging;

namespace McuTools
{
    /// <summary>
    /// Interaction logic for SmartHomeScreen.xaml
    /// </summary>
    public partial class SmartHomeScreen : UserControl, IFixedTool
    {
        private bool _loaded;
        private List<ToolBase> _tools;
        private enum Toolcat { Analog, Digital, Web, Other, External, All, Books, Favorites }
        private FileToIconConverter _fc;
        private BookManager _books;

        public MainWindow MainWin { get; set; }

        public SmartHomeScreen()
        {
            InitializeComponent();
            _tools = new List<ToolBase>(App._Tools.Count + App._ExtTools.Count + App._WebTools.Count + App._Popups.Count);
            _tools.AddRange(App._Tools);
            _tools.AddRange(App._WebTools);
            _tools.AddRange(App._ExtTools);
            _tools.AddRange(App._Popups);
            _fc = new FileToIconConverter();
            _books = new BookManager();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            _loaded = true;
            RenderItems(Toolcat.All, null, true);
        }

        private int RenderList(List<ToolBase> items)
        {
            int i = 0;
            foreach (var item in items)
            {
                ImageButton btn = new ImageButton();
                btn.Width = 120;
                btn.Height = 120;
                btn.ImageText = item.Description;
                if (item.Description.Length > 60) btn.ToolTip = item.Description;

                if (item.Icon == null) btn.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/attach-128.png", UriKind.Absolute));
                else btn.ImageSource = item.Icon;

                btn.Margin = new Thickness(5);
                btn.Name = "btn_" + i.ToString();
                btn.Click += new RoutedEventHandler(ToolClicked);
                if (item is ExternalTool)
                {
                    if ((item as ExternalTool).IsVisible == false) continue;
                }
                View.Children.Add(btn);
                i++;
            }
            return i;
        }

        private int RenderBooks(string search = null)
        {
            int i = 0;
            foreach (var book in _books.Filter(search))
            {
                ImageButton btn = new ImageButton();
                btn.Width = 120;
                btn.Height = 120;
                btn.ImageText = book;
                if (book.Length > 60) btn.ToolTip = book;
                btn.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/pdf-128.png", UriKind.Absolute));
                btn.Margin = new Thickness(5);
                btn.Name = "book_" + i.ToString();
                btn.Click += Book_Click;
                View.Children.Add(btn);
                i++;
            }
            return i;
        }

        void Book_Click(object sender, RoutedEventArgs e)
        {
            var d = sender.ToString();
            if (!BookManager.PDFReaderInstalled())
            {
                WpfHelpers.ExceptionDialog("No PDF Readers installed. To view documents a PDF reader must be installed");
                return;
            }
            string f = _books.GetFilePath(d);
            Process p = new Process();
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = f;
            p.Start();
            App._Config.UsageStats[d] += 1;
        }


        private void ToolClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                var d = sender.ToString();
                var tool = (from i in App._Tools where i.Description == d select i).FirstOrDefault();
                if (tool != null) MainWin.RunTool(tool);
                else
                {
                    var external = (from i in App._ExtTools where i.Description == d select i).FirstOrDefault();
                    if (external != null) MainWin.RunETool(external);
                    else
                    {
                        var web = (from i in App._WebTools where i.Description == d select i).FirstOrDefault();
                        if (web != null) MainWin.RunWebTool(web);
                        else
                        {
                            var pop = (from i in App._Popups where i.Description == d select i).FirstOrDefault();
                            if (pop != null) MainWin.RunPopupTool(pop);
                        }
                    }
                }
                App._Config.UsageStats[d] += 1;
            }
            catch (Exception ex)
            {
                WpfHelpers.ExceptionDialog(ex);
            }
        }

        public void RefreshView()
        {
            RenderItems(Toolcat.All);
        }

        private void RenderItems(Toolcat Category, string searchtext = null, bool noanim = false)
        {
            if (!_loaded) return;
            View.Children.Clear();
            List<ToolBase> list;

            if (!string.IsNullOrEmpty(searchtext))
            {
                list = (from i in _tools where i.Description.ToLower().Contains(searchtext.ToLower()) orderby i.Description ascending select i).ToList();
                RenderList(list);
                return;
            }

            if (!noanim) WpfHelpers.FindStoryBoard(this, "Change").Begin();
            int counter = 0;
            switch (Category)
            {
                case Toolcat.All:
                    list = (from i in _tools orderby i.Description ascending select i).ToList();
                    counter += RenderList(list);
                    Header.Text = string.Format("Tools ({0})", counter);
                    break;
                case Toolcat.Favorites:
                    var items = (from i in _tools where App._Config.UsageStats.ContainsKey(i.Description) orderby App._Config.UsageStats[i.Description] descending select i);
                    counter += RenderList(items.ToList());
                    counter += RenderBooks("**fav**");
                    Header.Text = string.Format("Most used ({0})", counter);
                    break;
                case Toolcat.Analog:
                    list = (from i in _tools where i.Category == ToolCategory.Analog orderby i.Description ascending select i).ToList();
                    counter += RenderList(list);
                    Header.Text = string.Format("Analog Tools ({0})", counter);
                    break;
                case Toolcat.Digital:
                    list = (from i in _tools where i.Category == ToolCategory.Digital orderby i.Description ascending select i).ToList();
                    counter += RenderList(list);
                    Header.Text = string.Format("Digital Tools ({0})", counter);
                    break;
                case Toolcat.Other:
                    list = (from i in _tools where i.Category == ToolCategory.Other orderby i.Description ascending select i).ToList();
                    counter += RenderList(list);
                    Header.Text = string.Format("Other Tools ({0})", counter);
                    break;
                case Toolcat.Web:
                    list = (from i in _tools where i.Category == ToolCategory.Web orderby i.Description ascending select i).ToList();
                    counter += RenderList(list);
                    Header.Text = string.Format("Web Tools ({0})", counter);
                    break;
                case Toolcat.External:
                    list = (from i in _tools where i.Category == ToolCategory.External orderby i.Description ascending select i).ToList();
                    counter += RenderList(list);
                    Header.Text = string.Format("External Programs ({0})", counter);
                    break;
                case Toolcat.Books:
                    counter += RenderBooks();
                    Header.Text = string.Format("Books ({0})", counter);
                    break;
            }
        }

        private void SwitchCategory(object sender, RoutedEventArgs e)
        {
            Button s = (Button)sender;
            Toolcat cat = (Toolcat)Enum.Parse(typeof(Toolcat), s.Name, true);
            RenderItems(cat);
            TbSearch.Text = "";
        }

        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            RenderItems(Toolcat.All, TbSearch.Text);
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TbSearch.Text)) return;
            TbSearch.Text = null;
            RenderItems(Toolcat.All);
        }

        private void ScrollView_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToHorizontalOffset(scv.HorizontalOffset - e.Delta);
            e.Handled = true;
        }

        private void ScrollView_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            TbSearch.Focus();
        }
    }
}
