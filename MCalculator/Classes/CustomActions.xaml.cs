using McuTools.Interfaces.WPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace MCalculator.Classes
{
    /// <summary>
    /// Interaction logic for CustomActions.xaml
    /// </summary>
    internal partial class CustomActions : UserControl
    {
        private List<UserCode> _codes;

        public CustomActions()
        {
            InitializeComponent();
            _codes = new List<UserCode>();
        }

        public Calculator2 MainForm { get; set; }

        private void Load(string serializeddata)
        {
            try
            {
                if (string.IsNullOrEmpty(serializeddata)) return;
                XmlSerializer ser = new XmlSerializer(typeof(UserCode));
                StringReader stringReader = new StringReader(serializeddata);
                UserCode[] loaded = (UserCode[])ser.Deserialize(stringReader);
                _codes.Clear();
                _codes.AddRange(loaded);
                loaded = null;
            }
            catch (Exception ex)
            {
                WpfHelpers.ExceptionDialog(ex);
            }
        }

        private string Save()
        {
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(UserCode));
                StringWriter textWriter = new StringWriter();
                ser.Serialize(textWriter, _codes.ToArray());
                return ser.ToString();
            }
            catch (Exception ex)
            {
                WpfHelpers.ExceptionDialog(ex);
                return null;
            }
        }

        private void CreateUI()
        {
            foreach (var c in _codes)
            {
                Button b = new Button();
                b.Margin = new Thickness(5);
                b.Height = 54;
                b.Width = 100;
                b.Content = c.FunctionName;
                b.Click += b_Click;
                Buttons.Children.Add(b);
            }
        }

        private void b_Click(object sender, RoutedEventArgs e)
        {
            string header = ((Button)sender).Content.ToString();

            if (RbEdit.IsChecked == true || RbExec.IsChecked == true)
            {
                var q = (from i in _codes where i.FunctionName == header select i.FunctionData).FirstOrDefault();
                MainForm.SynBox.Text = q;
                if (RbExec.IsChecked == true) MainForm.RunCommand();
            }
            else if (RbDelete.IsChecked == true)
            {
                var q = (from i in _codes where i.FunctionName == header select i).FirstOrDefault();
                _codes.Remove(q);
                CreateUI();
            }
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "User functions | *.func";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (var t = File.OpenText(ofd.FileName))
                {
                    Load(t.ReadToEnd());
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "User functions | *.func";
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (var t = File.CreateText(sfd.FileName))
                {
                    t.Write(Save());
                }
            }
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    [Serializable]
    public class UserCode
    {
        public string FunctionName { get; set; }
        public string FunctionData { get; set; }
    }
}
