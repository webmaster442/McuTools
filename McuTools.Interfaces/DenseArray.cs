using System.Collections.Generic;
using System;

namespace McuTools.Interfaces
{
    public class DenseArray<T>: IEnumerable<T>
    {
        private T[] _array;
        
        /// <summary>
        /// Creates a new instance of DenseArray
        /// </summary>
        /// <param name="rows">Number of rows</param>
        /// <param name="columns">Number of columns</param>
        public DenseArray(int rows, int columns)
        {
            _array = new T[rows * columns];
        }

        /// <summary>
        /// Creates a new instance of DenseArray
        /// </summary>
        /// <param name="array">source 2d array</param>
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

        /// <summary>
        /// Gets the number of columns
        /// </summary>
        public int Columns { get; private set; }

        /// <summary>
        /// Gets the number of rows
        /// </summary>
        public int Rows { get; private set; }

        /// <summary>
        /// Gets or sets an element of the array
        /// </summary>
        /// <param name="row">Row index</param>
        /// <param name="column">Column index</param>
        /// <returns>Vaalue at row and column index</returns>
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

        /// <summary>
        /// Gets a row as an Array 
        /// </summary>
        /// <param name="rowindex">Row index</param>
        /// <returns>Row values in an array</returns>
        public T[] GetRow(int rowindex)
        {
            T[] ret = new T[this.Columns];
            for (int i = 0; i < this.Columns; i++)
            {
                ret[i] = this[rowindex, i];
            }
            return ret;
        }

        /// <summary>
        /// Gets a column as an Array
        /// </summary>
        /// <param name="columnindex">Column index</param>
        /// <returns>Column values in an array</returns>
        public T[] GetColumn(int columnindex)
        {
            T[] ret = new T[this.Rows];
            for (int i = 0; i < this.Rows; i++ )
            {
                ret[i] = this[i, columnindex];
            }
            return ret;
        }

        /// <summary>
        /// Sets a row's values from an array
        /// </summary>
        /// <param name="row">the row index</param>
        /// <param name="items">items in an array</param>
        public void SetRow(int row, T[] items)
        {
            if (row < 0 || row > this.Rows) throw new ArgumentOutOfRangeException("row");
            if (items == null) throw new ArgumentNullException("items");

            int limit = Math.Min(items.Length, this.Columns);
            
            for (int i=0; i<limit; i++)
            {
                this[row, i] = items[i];
            }
        }

        /// <summary>
        /// Sets a column's values from an array
        /// </summary>
        /// <param name="column">the column index</param>
        /// <param name="items">items in an array</param>
        public void SetColumn(int column, T[] items)
        {
            if (column < 0 || column > this.Columns) throw new ArgumentOutOfRangeException("column");
            if (items == null) throw new ArgumentNullException("items");

            int limit = Math.Min(items.Length, this.Rows);

            for (int i=0; i<limit; i++)
            {
                this[i, column] = items[i];
            }
        }
    }
}
