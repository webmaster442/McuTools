using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace McuTools.Interfaces
{
    public class DenseArray<T>: IEnumerable<T>
    {
        private T[] _array;

        public DenseArray(int rows, int columns)
        {
            _array = new T[rows * columns];
        }

        public DenseArray(T[,] array)
        {
            _array = new T[array.GetLength(0) * array.GetLength(1)];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.Length; j++)
                {
                    _array[j * Columns + i] = array[i, j];
                }
            }
        }

        public int Columns { get; private set; }

        public int Rows { get; private set; }

        public T this[int row, int column]
        {
            get { return _array[column * Columns + row]; }
            set { _array[column * Columns + row] = value; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (IEnumerator<T>)_array.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _array.GetEnumerator();
        }
    }
}
