using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MTools.classes
{
    internal class StatSampling : List<short>, INotifyPropertyChanged
    {
        public StatSampling() : base() { }

        public double VoltsPerItem
        {
            get;
            set;
        }

        public string MapToVolts(double input)
        {
            double volts = input * VoltsPerItem;
            return string.Format("{0:0.####} - {1:0.0000} V", input, volts);
        }

        public string Maximum
        {
            get
            {
                if (this.Count > 0) return MapToVolts(this.Max());
                else return MapToVolts(0);
            }
        }

        public string Minimum
        {
            get
            {
                if (this.Count > 0) return MapToVolts(this.Min());
                else return MapToVolts(0);
            }
        }

        public string Average
        {
            get
            {
                if (this.Count > 0) return MapToVolts(this.Average(x => x));
                else return MapToVolts(0);
            }
        }

        public string Range
        {
            get
            {
                if (this.Count > 0) return MapToVolts(this.Max() - this.Min());
                else return MapToVolts(0);
            }
        }

        /*public new void Add(short item)
        {
            base.Add(item);
            FirePropertyChanged("Maximum");
            FirePropertyChanged("Minimum");
            FirePropertyChanged("Average");
            FirePropertyChanged("Range");
            FirePropertyChanged("Count");
        }*/

        public string Add(short item)
        {
            base.Add(item);
            FirePropertyChanged("Maximum");
            FirePropertyChanged("Minimum");
            FirePropertyChanged("Average");
            FirePropertyChanged("Range");
            FirePropertyChanged("Count");
            return MapToVolts(item);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void FirePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
