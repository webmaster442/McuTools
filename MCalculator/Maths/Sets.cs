using IronPython.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Double;

namespace MCalculator.Maths
{
    /// <summary>
    /// Set type
    /// </summary>
    public class Set : List<double>
    {
        /// <summary>
        /// Creates an empty set
        /// </summary>
        public Set() : base() { }

        /// <summary>
        /// Creates an empty set, that is capable of storing at least the given number of items.
        /// </summary>
        /// <param name="items">Number of items to be capeable to store at least</param>
        public Set(int items) : base(items) { }

        /// <summary>
        /// Creates a set from a nother set, or an array, or any type that is enumerable.
        /// </summary>
        /// <param name="Collection">Initial items as an Ienumerable collection</param>
        public Set(IEnumerable<double> Collection) : base(Collection) { }

        /// <summary>
        /// Clones the current set
        /// </summary>
        public Set Clone()
        {
            return new Set((from i in this select i).ToList());
        }

        /// <summary>
        /// Converts set to a vector
        /// </summary>
        /// <param name="s">The set to convert</param>
        public static DenseVector ToVector(Set s)
        {
            return DenseVector.OfEnumerable(s);
        }

        /// <summary>
        /// Converts a vector to a set
        /// </summary>
        /// <param name="v">the vector to convert</param>
        public static Set FromVector(DenseVector v)
        {
            return new Set(v);
        }

        /// <summary>
        /// Vector addition of sets
        /// </summary>
        /// <param name="one">A set</param>
        /// <param name="two">A nother set</param>
        public static Set operator +(Set one, Set two)
        {
            if (one.Count != two.Count) throw new ArgumentException("Sets can't be added if the element number doesn't match");
            Set ret = new Set(one.Count);
            Parallel.For(0, ret.Count, i =>
            {
                ret[i] = one[i] + two[i];
            });
            return ret;
        }

        /// <summary>
        /// Vector subtraction of two sets
        /// </summary>
        /// <param name="one">A set</param>
        /// <param name="two">A nother set</param>
        public static Set operator -(Set one, Set two)
        {
            if (one.Count != two.Count) throw new ArgumentException("Sets can't be subtracted if the element number doesn't match");
            Set ret = new Set(one.Count);
            Parallel.For(0, ret.Count, i =>
            {
                ret[i] = one[i] - two[i];
            });
            return ret;
        }

        /// <summary>
        /// Vector dot product of sets
        /// </summary>
        /// <param name="one">A set</param>
        /// <param name="two">A nother set</param>
        public static double operator *(Set one, Set two)
        {
            DenseVector v1 = Set.ToVector(one);
            DenseVector v2 = Set.ToVector(two);
            return v1.DotProduct(v2);
        }

        /// <summary>
        /// Vector division of sets
        /// </summary>
        /// <param name="one">A set</param>
        /// <param name="two">A nother set</param>
        public static Set operator /(Set one, Set two)
        {
            DenseVector v1 = Set.ToVector(one);
            DenseVector v2 = Set.ToVector(two);
            return Set.FromVector((DenseVector)(v1 / v2));
        }

        /// <summary>
        /// Adds a number to the items of the set
        /// </summary>
        /// <param name="one">A set</param>
        /// <param name="two">A number to add</param>
        public static Set operator +(Set one, double two)
        {
            Set ret = new Set(one.Count);
            Parallel.For(0, ret.Count, i =>
            {
                ret[i] = one[i] + two;
            });
            return ret;
        }

        /// <summary>
        /// Subtracts a number from the elements of the set
        /// </summary>
        /// <param name="one">A set</param>
        /// <param name="two">A number</param>
        public static Set operator -(Set one, double two)
        {
            Set ret = new Set(one.Count);
            Parallel.For(0, ret.Count, i =>
            {
                ret[i] = one[i] - two;
            });
            return ret;
        }

        /// <summary>
        /// Multiplies elements of a sets with a number
        /// </summary>
        /// <param name="one">A number</param>
        /// <param name="two">multiplicatior</param>
        public static Set operator *(Set one, double two)
        {
            Set ret = new Set(one.Count);
            Parallel.For(0, ret.Count, i =>
            {
                ret[i] = one[i] * two;
            });
            return ret;
        }

        /// <summary>
        /// Divides the elements of a set with a number
        /// </summary>
        /// <param name="one">A number</param>
        /// <param name="two">divisior</param>
        public static Set operator /(Set one, double two)
        {
            Set ret = new Set(one.Count);
            Parallel.For(0, ret.Count, i =>
            {
                ret[i] = one[i] / two;
            });
            return ret;
        }
    }

    /// <summary>
    /// Set functions
    /// </summary>
    public static class Sets
    {
        /// <summary>
        /// Creates a set from the specified elements
        /// </summary>
        /// <param name="pars">elements of the set</param>
        public static Set Create(params double[] pars)
        {
            return new Set(pars);
        }

        /// <summary>
        /// Creates a set from a python list
        /// </summary>
        /// <param name="list">a list containing the elements of the set</param>
        public static Set FromList(List list)
        {
            Set ret = new Set();
            foreach (var item in list) ret.Add(Convert.ToDouble(item));
            return ret;
        }

        /// <summary>
        /// Appends numbers to an existing set
        /// </summary>
        /// <param name="set">A set to apend to</param>
        /// <param name="pars">elements to append</param>
        public static void Append(Set set, params double[] pars)
        {
            set.AddRange(pars);
        }

        /// <summary>
        /// Appends python lists to an existing set
        /// </summary>
        /// <param name="set">a set to apend to</param>
        /// <param name="lists">python sets to apend</param>
        public static void AppendList(Set set, params List[] lists)
        {
            foreach (var list in lists)
            {
                foreach (var item in list)
                {
                    set.Add(Convert.ToDouble(item));
                }
            }
        }

        /// <summary>
        /// The union of A and B, denoted by A ∪ B, is the set of all things that are members of either A or B.
        /// </summary>
        /// <param name="a">A set of numbers</param>
        /// <param name="b">A set of numbers</param>
        public static Set Union(Set a, Set b)
        {
            Set ret = new Set();
            foreach (var item in b)
            {
                if (!ret.Contains(item)) ret.Add(item);
            }
            foreach (var item in a)
            {
                if (!ret.Contains(item)) ret.Add(item);
            }
            return ret;
        }

        /// <summary>
        /// Merges two sets.
        /// </summary>
        /// <param name="a">A set of numbers</param>
        /// <param name="b">A set of numbers</param>
        public static Set Merge(Set a, Set b)
        {
            Set ret = new Set();
            ret.AddRange(a);
            ret.AddRange(b);
            return ret;
        }

        /// <summary>
        /// Subtracts set b from a. Result set is the set of all elements that are members of A, but not members of B. 
        /// </summary>
        /// <param name="a">A set of numbers</param>
        /// <param name="b">A set of numbers</param>
        public static Set Complement(Set a, Set b)
        {
            Set ret = new Set();
            foreach (var item in a)
            {
                if (!b.Contains(item)) ret.Add(item);
            }
            return ret;
        }

        /// <summary>
        /// The intersection of A and B, denoted by A ∩ B, is the set of all things that are members of both A and B. If A ∩ B = ∅, then A and B are said to be disjoint.
        /// </summary>
        /// <param name="a">A set of numbers</param>
        /// <param name="b">A set of numbers</param>
        public static Set Intersection(Set a, Set b)
        {
            Set ret = new Set();
            foreach (var item in a)
            {
                if (b.Contains(item)) ret.Add(item);
            }
            return ret;
        }

        /// <summary>
        /// The Complement of Union(a, b) and Intersection(a, b)
        /// </summary>
        /// <param name="a">A set of numbers</param>
        /// <param name="b">A set of numbers</param>
        public static Set SymmetricDiff(Set a, Set b)
        {
            return Complement(Union(a, b), Intersection(a, b));
        }

        /// <summary>
        /// Returns the number of elements found in a aset
        /// </summary>
        /// <param name="a">A set of numbers</param>
        public static double Count(Set a)
        {
            return a.Count;
        }

        /// <summary>
        /// Returns true, if an item is present at a given set.
        /// </summary>
        /// <param name="set">A set of numbers</param>
        /// <param name="item">Item to search</param>
        public static bool ContainsItem(Set set, double item)
        {
            return set.Contains(item);
        }

        /// <summary>
        /// Sorts the set's items. If no parameter given or the parameter is false, then items are sorted in ascending, if parameter is true then descending.
        /// </summary>
        /// <param name="s">>A set of numbers</param>
        /// <param name="descending">If it's false, then items are sorted in ascending, if it's true then descending.</param>
        public static Set Sort(Set s, bool descending = false)
        {
            Set sorted = null;
            if (descending == false) sorted = new Set((from i in s orderby i ascending select i).ToList());
            else sorted = new Set((from i in s orderby i descending select i).ToList());
            return sorted;
        }

        /// <summary>
        /// Applies a function transformation on all of the set's items
        /// </summary>
        /// <param name="s">A set of numbers</param>
        /// <param name="f">A function that will be used for transformation. Function takes one parameter and returns a number</param>
        public static Set Transform(Set s, OneParamFunction f)
        {
            Set ret = new Set();
            for (int i = 0; i < s.Count; i++)
            {
                ret.Add(f(s[i]));
            }
            return ret;
        }
    }
}
