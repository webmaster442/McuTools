using McuTools.Interfaces.WPF;
using MTools.classes;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace MTools.ToolOther
{
    /// <summary>
    /// Interaction logic for Crypto.xaml
    /// </summary>
    public partial class Crypto : UserControl
    {
        private Progress<double> Indicator;
        private bool _loaded;
        private CancellationTokenSource cts;

        public Crypto()
        {
            InitializeComponent();
            Indicator = new Progress<double>(ReportProgress);
        }

        private void ReportProgress(double obj)
        {
            PbProgress.Value = obj;
        }

        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            BtnStart.IsEnabled = false;
            Modeselect.IsEnabled = false;
            cts = new CancellationTokenSource();
            try
            {
                switch (Modeselect.SelectedIndex)
                {
                    case 0:
                        TbOutput.Clear();
                        string specials = null;
                        string gen = null;
                        if ((bool)CbSpecial.IsChecked) specials = TbSpecials.Text;
                        for (int i = 0; i < SlNumber.Value; i++)
                        {
                            gen = Cryptog.GeneratePassWord((bool)CbLowercase.IsChecked, (bool)CbLowercase.IsChecked, (bool)CbNumbers.IsChecked, specials, (int)SlLength.Value);
                            TbOutput.AppendText(gen + "\r\n");
                        }
                        break;
                    case 1:
                        TbCaesarOutput.Clear();

                        if (RbCaesarEnc.IsChecked == true)
                        {
                            var rules = CaesarRules.Classic;
                            if (RbCaesarRandom.IsChecked == true) rules = CaesarRules.Random;
                            TbCaesarKeyRule.Text = CaesarRules.SerializeRule(rules);
                            TbCaesarOutput.Text = Cryptog.CaesarCrypt(TbCaesarInput.Text, rules, false);
                        }
                        else
                        {
                            var rules = CaesarRules.DeserializeRule(TbCaesarKeyRule.Text);
                            TbCaesarOutput.Text = Cryptog.CaesarCrypt(TbCaesarInput.Text, rules, true);
                        }

                        break;
                    case 2:
                        HashAlgorithms alg = HashAlgorithms.MD5;
                        if ((bool)RbMD5.IsChecked) alg = HashAlgorithms.MD5;
                        else if ((bool)RbSHA1.IsChecked) alg = HashAlgorithms.SHA1;
                        else if ((bool)RbSHA256.IsChecked) alg = HashAlgorithms.SHA256;
                        else if ((bool)RbSHA512.IsChecked) alg = HashAlgorithms.SHA512;

                        string hash = await Cryptog.ComputeHashTask(cts.Token, FInput.SelectedPath, Indicator, alg);
                        TbHashOutput.Text = hash;
                        break;
                    case 3:
                        await Cryptog.XorEncryptTask(cts.Token, XorIn.SelectedPath, XorKey.SelectedPath, XorOut.SelectedPath, Indicator);
                        break;
                    case 4:
                        KeySizeAES keysize = KeySizeAES.bit128;
                        if ((bool)RbAesK128.IsChecked) keysize = KeySizeAES.bit128;
                        else if ((bool)RbAesK192.IsChecked) keysize = KeySizeAES.bit192;
                        else if ((bool)RbAesK256.IsChecked) keysize = KeySizeAES.bit256;

                        if ((bool)RbAesEnc.IsChecked) TbAesKey.Text = await Cryptog.AesEncryptTask(cts.Token, AesIn.SelectedPath, AesOut.SelectedPath, keysize, Indicator);
                        else if ((bool)RbAesDec.IsChecked) await Cryptog.AesDecryptTask(cts.Token, AesIn.SelectedPath, AesOut.SelectedPath, TbAesKey.Text, Indicator);
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                WpfHelpers.ExceptionDialog("Task Canceled");
            }
            Modeselect.IsEnabled = true;
            BtnStart.IsEnabled = true;
        }

        private void Encryptmode_Checked(object sender, RoutedEventArgs e)
        {
            if (!_loaded) return;
            if ((bool)Encryptmode.IsChecked) XorKey.DialogType = McuTools.Interfaces.Controls.DialogType.SaveFile;
            else XorKey.DialogType = McuTools.Interfaces.Controls.DialogType.OpenFile;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_loaded) return;
            Encryptmode_Checked(null, null);
            _loaded = true;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel();
                PbProgress.Value = 0;
                BtnStart.IsEnabled = true;
            }
        }
    }
}
