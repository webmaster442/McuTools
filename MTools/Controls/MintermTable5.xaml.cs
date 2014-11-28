using MTools.classes;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MTools.Controls
{
    /// <summary>
    /// Interaction logic for MintermTable5.xaml
    /// </summary>
    public partial class MintermTable5 : UserControl, IMintermTable
    {
        public MintermTable5()
        {
            InitializeComponent();
        }

        public List<classes.LogicItem> Selected
        {
            get
            {
                List<LogicItem> ret = new List<LogicItem>();
                foreach (var item in Functions.GetMintermTableValues(Minterm5x))
                {
                    ret.Add(new LogicItem() { Index = item.Key, Checked = item.Value, BinaryValue = LogicItem.GetBinaryValue(item.Key, 5) });
                }
                return ret;
            }
            set { Functions.SetMintermTableValues(Minterm5x, value); }
        }


        public void ClearInput()
        {
            Functions.ClearMintermtable(Minterm5x);
        }

        public void SwapVarnames()
        {
            foreach (var tb in McuTools.Interfaces.WPF.WpfHelpers.FindChildren<TextBlock>(this))
            {
                switch (tb.Text)
                {
                    case "A":
                        tb.Text = "E";
                        break;
                    case "B":
                        tb.Text = "D";
                        break;
                    case "C":
                        tb.Text = "C";
                        break;
                    case "D":
                        tb.Text = "B";
                        break;
                    case "E":
                        tb.Text = "A";
                        break;
                }
            }
        }
    }
}
