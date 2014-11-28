using MTools.classes;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MTools.Controls
{
    /// <summary>
    /// Interaction logic for MintermTable.xaml
    /// </summary>
    public partial class MintermTable4 : UserControl, IMintermTable
    {
        public MintermTable4()
        {
            InitializeComponent();
        }

        public List<classes.LogicItem> Selected
        {
            get
            {
                List<LogicItem> ret = new List<LogicItem>();
                foreach (var item in Functions.GetMintermTableValues(Minterm4x))
                {
                    ret.Add(new LogicItem() { Index = item.Key, Checked = item.Value, BinaryValue = LogicItem.GetBinaryValue(item.Key, 4) });
                }
                return ret;
            }
            set { Functions.SetMintermTableValues(Minterm4x, value); }
        }


        public void ClearInput()
        {
            Functions.ClearMintermtable(Minterm4x);
        }

        public void SwapVarnames()
        {
            foreach (var tb in McuTools.Interfaces.WPF.WpfHelpers.FindChildren<TextBlock>(this))
            {
                switch (tb.Text)
                {
                    case "A":
                        tb.Text = "D";
                        break;
                    case "B":
                        tb.Text = "C";
                        break;
                    case "C":
                        tb.Text = "B";
                        break;
                    case "D":
                        tb.Text = "A";
                        break;
                }
            }
        }
    }
}
