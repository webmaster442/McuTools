using Be.Windows.Forms;
using McuTools.Interfaces.WPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MTools.Controls
{
    /// <summary>
    /// Interaction logic for HexEditor.xaml
    /// </summary>
    public partial class HexEditor : UserControl
    {
        public HexEditor()
        {
            InitializeComponent();
        }

        public void CreateDefault()
        {
            byte[] array = new byte[] { 0x00 };
            DynamicByteProvider byteprov = new DynamicByteProvider(array);
            hexBox.ByteProvider = byteprov;
        }

        public byte[] GetBytes()
        {
            if (hexBox.ByteProvider == null) return null;

            List<byte> buffer = new List<byte>();
            for (int i = 0; i < hexBox.ByteProvider.Length; i++)
            {
                buffer.Add(hexBox.ByteProvider.ReadByte(i));
            }
            return buffer.ToArray();
        }

        public void Load(string fileName)
        {
            if (!File.Exists(fileName))
            {
                MessageBox.Show("File not found: " + fileName, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                DynamicFileByteProvider dynamicFileByteProvider;
                try
                {
                    // try to open in write mode
                    dynamicFileByteProvider = new DynamicFileByteProvider(fileName);
                }
                catch (IOException) // write mode failed
                {
                    try
                    {
                        // try to open in read-only mode
                        dynamicFileByteProvider = new DynamicFileByteProvider(fileName, true);
                        if (MessageBox.Show("File is readonly. Open in reafonly mode?", "Question", MessageBoxButton.YesNo) == MessageBoxResult.No)
                        {
                            dynamicFileByteProvider.Dispose();
                            return;
                        }
                    }
                    catch (IOException) // read-only also failed
                    {
                        // file cannot be opened
                        MessageBox.Show("File open failed", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                hexBox.ByteProvider = dynamicFileByteProvider;
            }
            catch (Exception ex)
            {
                WpfHelpers.ExceptionDialog(ex);
            }
        }

        public void Save(string file)
        {
            if (hexBox.ByteProvider == null) return;
            try
            {
                DynamicFileByteProvider dynamicFileByteProvider = hexBox.ByteProvider as DynamicFileByteProvider;
                Stream target = File.Create(file);

                using (BinaryWriter bw = new BinaryWriter(target))
                {
                    for (int i = 0; i < dynamicFileByteProvider.Length; i++)
                    {
                        bw.Write(dynamicFileByteProvider.ReadByte(i));
                    }
                }
            }
            catch (Exception ex)
            {
                WpfHelpers.ExceptionDialog(ex);
            }
        }

        public event KeyEventHandler EditorKeyDown;

        private void MenNew_Click(object sender, RoutedEventArgs e)
        {
            NewHex nh = new NewHex();
            if (nh.ShowDialog() == false) return;

            byte[] array = new byte[nh.FileSize];
            DynamicByteProvider byteprov = new DynamicByteProvider(array);
            hexBox.ByteProvider = byteprov;
        }

        private void MenLoad_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "*.* | All files";
            ofd.FilterIndex = 0;
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Load(ofd.FileName);
            }
        }

        private void MenSave_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
            sfd.Filter = "*.* | All files";
            sfd.FilterIndex = 0;

            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Save(sfd.FileName);
            }
        }

        private void hexBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            Key wpfkey = KeyInterop.KeyFromVirtualKey((int)e.KeyCode);
            if (EditorKeyDown != null) EditorKeyDown(sender, new KeyEventArgs(Keyboard.PrimaryDevice, new FakePresentationSource(), 0, wpfkey));
        }
    }

    public class FakePresentationSource : PresentationSource
    {
        protected override CompositionTarget GetCompositionTargetCore()
        {
            return null;
        }

        public override Visual RootVisual { get; set; }

        public override bool IsDisposed { get { return false; } }
    }
}
