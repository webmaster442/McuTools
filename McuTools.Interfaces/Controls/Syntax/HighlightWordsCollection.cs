using System;
using System.Collections;
using System.Collections.Generic;

namespace McuTools.Interfaces.Controls.Syntax
{

    public class HighlightWordsCollection : IList<string>
    {
        private IList<string> internalList;

     

        public event EventHandler ListChanged;
        public event EventHandler ListCleared;

        public HighlightWordsCollection()
        {
            internalList = new List<string>();
        }

        public HighlightWordsCollection(IList<string> list)
        {
            internalList = list;
        }

        public HighlightWordsCollection(IEnumerable<string> collection)
        {
            internalList = new List<string>(collection);
        }

        protected virtual void OnListChanged(EventArgs e)
        {
            if (ListChanged != null)
                ListChanged(this, e);
        }

        protected virtual void OnListCleared(EventArgs e)
        {
            if (ListCleared != null)
                ListCleared(this, e);
        }

        public int IndexOf(string item)
        {
            return internalList.IndexOf(item);
        }

        public void Insert(int index, string item)
        {
            internalList.Insert(index, item);
            OnListChanged(EventArgs.Empty);
        }

        public void RemoveAt(int index)
        {
            string item = internalList[index];
            internalList.Remove(item);
            OnListChanged(EventArgs.Empty);
        }

        public string this[int index]
        {
            get { return internalList[index]; }
            set
            {
                internalList[index] = value;
                OnListChanged(EventArgs.Empty);
            }
        }

        public void Add(string item)
        {
            internalList.Add(item);
            OnListChanged(EventArgs.Empty); 
        } 

        public void Clear()
        {
            internalList.Clear();
            OnListCleared(new EventArgs());
        }

        public bool Contains(string item)
        {
            return internalList.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return internalList.Count; }
        }

        public bool IsReadOnly
        {
            get { return IsReadOnly; }
        }

        public bool Remove(string item)
        {
            lock (this)
            {
                int index = internalList.IndexOf(item);
                if (internalList.Remove(item))
                {
                    OnListChanged(EventArgs.Empty);
                    return true;
                }
                else
                    return false;
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return internalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)internalList).GetEnumerator();
        }
    }
}

