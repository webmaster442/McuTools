using MTools.classes;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MTools.Controls
{
    /// <summary>
    /// Interaction logic for MintermTable2.xaml
    /// </summary>
    public partial class MintermTable2 : UserControl, IMintermTable
    {
        public MintermTable2()
        {
            InitializeComponent();
        }

        public List<LogicItem> Selected
        {
            get
            {
                List<LogicItem> ret = new List<LogicItem>();
                foreach (var item in Functions.GetMintermTableValues(Minterm2x))
                {
                    ret.Add(new LogicItem() { Index = item.Key, Checked = item.Value, BinaryValue = LogicItem.GetBinaryValue(item.Key, 2) });
                }
                return ret;
            }
            set{ Functions.SetMintermTableValues(Minterm2x, value); }
        }


        public void ClearInput()
        {
            Functions.ClearMintermtable(Minterm2x);
        }

        public void SwapVarnames()
        {
            foreach (var tb in McuTools.Interfaces.WPF.WpfHelpers.FindChildren<TextBlock>(this))
            {
                switch (tb.Text)
                {
                    case "A":
                        tb.Text = "B";
                        break;
                    case "B":
                        tb.Text = "A";
                        break;
                }
            }
        }
    }

    public interface IMintermTable
    {
        List<LogicItem> Selected { get; set; }
        void ClearInput();
        void SwapVarnames();
    }
}
