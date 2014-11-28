using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTools.classes
{
    public class LogicItem : INotifyPropertyChanged
    {
        private bool? _Checked;
        private int _Index;
        private string _BinaryValue;

        public bool? Checked
        {
            get { return _Checked; }
            set 
            {
                _Checked = value;
                FirePropertyChangedEvent("Checked");
            }
        }

        public string BinaryValue
        {
            get { return _BinaryValue; }
            set
            {
                _BinaryValue = value;
                FirePropertyChangedEvent("BinaryValue");
            }
        }

        public int Index
        {
            get { return _Index; }
            set
            {
                _Index = value;
                FirePropertyChangedEvent("Index");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void FirePropertyChangedEvent(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public static string GetBinaryValue(int number, int chars)
        {
            string bin = Convert.ToString(number, 2);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < chars - bin.Length; i++)
            {
                sb.Append("0");
            }
            sb.Append(bin);
            return sb.ToString();
        }
    }
}
