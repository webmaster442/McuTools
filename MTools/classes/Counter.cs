using System.ComponentModel;

namespace MTools.classes
{
    public class Counter : INotifyPropertyChanged
    {
        private string _current, _next;

        public string Current
        {
            get { return _current; }
            set
            {
                _current = value;
                FirePropertyChangedEvent("Current");
            }
        }

        public string Next
        {
            get { return _next; }
            set
            {
                _next = value;
                FirePropertyChangedEvent("Next");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void FirePropertyChangedEvent(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
