using IronPython.Runtime;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MCalculator.Maths
{
    /// <summary>
    /// Matrix functions
    /// </summary>
    public static class Matrices
    {
        /// <summary>
        /// Creates a Matrix
        /// </summary>
        /// <param name="rows">number of matrix rows</param>
        /// <param name="columns">number of matrix columns</param>
        public static DenseMatrix CreateMatrix(int rows, int columns)
        {
            return new DenseMatrix(rows, columns);
        }

        private static double[] ConvertList(List l)
        {
            List<double> d = new List<double>();
            foreach (var item in l) d.Add(Convert.ToDouble(item));
            return d.ToArray();
        }

        /// <summary>
        /// Creates a matrix by specifying it's rows
        /// </summary>
        /// <param name="rows">Rows specified by python lists</param>
        public static DenseMatrix CreateFromRows(params List[] rows)
        {
            var q1 = (from item in rows select item.Count);
            if (q1.Max() != q1.Min()) throw new Exception("List item count doesn't mach for all lists");
            DenseMatrix matrix = new DenseMatrix(rows.Length, rows[0].Count);
            int j=0;
            foreach (var row in rows)
            {
                matrix.SetRow(j, ConvertList(row));
                j++;
            }
            return matrix;
        }

        /// <summary>
        /// Creates a matrix by specifying it's rows
        /// </summary>
        /// <param name="rows">Rows specified by Sets</param>
        public static DenseMatrix CreateFromRows(params Set[] rows)
        {
            var q1 = (from item in rows select item.Count);
            if (q1.Max() != q1.Min()) throw new Exception("Set item count doesn't mach for all lists");
            DenseMatrix matrix = new DenseMatrix(rows.Length, rows[0].Count);
            int j=0;
            foreach (var row in rows)
            {
                matrix.SetRow(j, row.ToArray());
                j++;
            }
            return matrix;
        }

        /// <summary>
        /// Creates a matrix by specifying it's columns
        /// </summary>
        /// <param name="columns">Columns specified by python lists</param>
        public static DenseMatrix CreateFromColumns(params List[] columns)
        {
            var q1 = (from item in columns select item.Count);
            if (q1.Max() != q1.Min()) throw new Exception("List item count doesn't mach for all lists");
            DenseMatrix matrix = new DenseMatrix(columns[0].Count, columns.Length);
            int j = 0;
            foreach (var column in columns)
            {
                matrix.SetColumn(j, ConvertList(column));
                j++;
            }
            return matrix;
        }

        /// <summary>
        /// Creates a matrix by specifying it's columns
        /// </summary>
        /// <param name="columns">Columns specified by sets</param>
        public static DenseMatrix CreateFromColumns(params Set[] columns)
        {
            var q1 = (from item in columns select item.Count);
            if (q1.Max() != q1.Min()) throw new Exception("List item count doesn't mach for all lists");
            DenseMatrix matrix = new DenseMatrix(columns[0].Count, columns.Length);
            int j = 0;
            foreach (var column in columns)
            {
                matrix.SetColumn(j, column.ToArray());
                j++;
            }
            return matrix;
        }

        /// <summary>
        /// Sets a row of a matrix by giving row valuews
        /// </summary>
        /// <param name="matrix">The matrix to be modified</param>
        /// <param name="row">Index of the row that will be modified</param>
        /// <param name="items">Row items</param>
        public static void SetRow(DenseMatrix matrix, int row, params double[] items)
        {
            if (items.Length != matrix.ColumnCount) throw new Exception(string.Format("item count mismatch. Expected {0} items, got {1} items instead.", matrix.ColumnCount, items.Length));
            matrix.SetRow(row, items);
        }

        /// <summary>
        /// Sets a row of a matrix from a set
        /// </summary>
        /// <param name="matrix">The matrix to be modified</param>
        /// <param name="row">Index of the row that will be modified</param>
        /// <param name="items">A set</param>
        public static void SetRow(DenseMatrix matrix, int row, Set items)
        {
            if (items.Count != matrix.ColumnCount) throw new Exception(string.Format("item count mismatch. Expected {0} items, got {1} items instead.", matrix.ColumnCount, items.Count));
            matrix.SetRow(row, items.ToArray());
        }

        /// <summary>
        /// Sets a column of a Matrix by giving column valuews
        /// </summary>
        /// <param name="matrix">The matrix to be modified</param>
        /// <param name="column">Index of the column that will be modified</param>
        /// <param name="items">Column items</param>
        public static void SetColumn(DenseMatrix matrix, int column, params double[] items)
        {
            if (items.Length != matrix.RowCount) throw new Exception(string.Format("item count mismatch. Expected {0} items, got {1} items instead.", matrix.RowCount, items.Length));
            matrix.SetColumn(column, items);
        }

        /// <summary>
        /// Sets a column of a matrix from a set
        /// </summary>
        /// <param name="matrix">The matrix to be modified</param>
        /// <param name="column">Index of the column that will be modified</param>
        /// <param name="items">A set</param>
        public static void SetColumn(DenseMatrix matrix, int column, Set items)
        {
            if (items.Count != matrix.RowCount) throw new Exception(string.Format("item count mismatch. Expected {0} items, got {1} items instead.", matrix.RowCount, items.Count));
            matrix.SetColumn(column, items.ToArray());
        }

        /// <summary>
        /// Returns a row of a matrix in a set
        /// </summary>
        /// <param name="matrix">Source matrix</param>
        /// <param name="row">index of the row</param>
        public static Set Row2Set(DenseMatrix matrix, int row)
        {
            var r = matrix.Row(row);
            return new Set(r.ToArray());
        }

        /// <summary>
        /// Returns a column of a matrix in a set
        /// </summary>
        /// <param name="matrix">Source Matrix</param>
        /// <param name="column">index of the column</param>
        /// 
        public static Set Column2Set(DenseMatrix matrix, int column)
        {
            var c = matrix.Column(column);
            return new Set(c.ToArray());
        }

        /// <summary>
        /// Computes the inverse of a matrix.
        /// </summary>
        /// <param name="matrix">a matrix</param>
        public static DenseMatrix Inverse(DenseMatrix matrix)
        {
            return DenseMatrix.OfMatrix(matrix.Inverse());
        }

        /// <summary>
        /// Computes the determinant of a matrix.
        /// </summary>
        /// <param name="matrix">a matrix</param>
        public static double Determinant(DenseMatrix matrix)
        {
            return matrix.Determinant();
        }

        /// <summary>
        /// Returns the transpose of a matrix.
        /// </summary>
        /// <param name="matrix">a matrix</param>
        public static DenseMatrix Transpose(DenseMatrix matrix)
        {
            return DenseMatrix.OfMatrix(matrix.Transpose());
        }

        /// <summary>
        /// Applies a function transformation on the items of a matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="f">A function that will be used for transformation. Function takes one parameter and returns a number</param>
        public static DenseMatrix Transform(DenseMatrix matrix, OneParamFunction f)
        {
            DenseMatrix ret = new DenseMatrix(matrix.RowCount, matrix.ColumnCount);
            for (int i = 0; i < ret.RowCount; i++)
            {
                for (int j = 0; j < ret.ColumnCount; j++)
                {
                    ret[i, j] = f(matrix[i, j]);
                }
            }
            return ret;
        }

    }
}
