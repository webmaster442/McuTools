using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCalculator.Maths
{
    /// <summary>
    /// Statistical functions
    /// </summary>
    public static class Stat
    {
        /// <summary>
        /// Returns the smallst vallue found in a set of numbers
        /// </summary>
        /// <param name="set">A set of numbers</param>
        public static double Minimum(Set set)
        {
            return set.AsParallel().Min();
        }

        /// <summary>
        /// Returns the bigest vallue found in a set of numbers
        /// </summary>
        /// <param name="set">A set of numbers</param>
        public static double Maximum(Set set)
        {
            return set.AsParallel().Max();
        }

        /// <summary>
        /// Returns the average vallue of a number set
        /// </summary>
        /// <param name="set">A set of numbers</param>
        public static double Average(Set set)
        {
            return set.AsParallel().Average();
        }

        /// <summary>
        /// Returns the geometric average vallue of a number set
        /// </summary>
        /// <param name="set">A set of numbers</param>
        public static double GeometricAvg(Set set)
        {
            double avg = 1.0;
            foreach (var itm in set) avg *= itm;
            return Math.Pow(avg, 1 / set.Count);
        }

        /// <summary>
        /// Returns the harmonic average vallue of a number set
        /// </summary>
        /// <param name="set">A set of numbers</param>
        public static double HarmonicAvg(Set set)
        {
            double avg = 0.0;
            foreach (var itm in set) avg += (1 / itm);
            return set.Count / avg;
        }

        /// <summary>
        /// Returns the square average vallue of a number set
        /// </summary>
        /// <param name="set">A set of numbers</param>
        public static double SquareAvg(Set set)
        {
            double avg = 0.0;
            foreach (var itm in set) avg += Math.Pow(itm, 2);
            return avg / set.Count;
        }

        /// <summary>
        /// Returns the sumarized value of a number set
        /// </summary>
        /// <param name="set">A set of numbers</param>
        public static double Sum(Set set)
        {
            return set.AsParallel().Sum();
        }

        /// <summary>
        /// Returns the range of a number set
        /// </summary>
        /// <param name="set">A set of numbers</param>
        public static double Range(Set set)
        {
            return Maximum(set) - Minimum(set);
        }

        /// <summary>
        /// Returns the difference between the value of an observation and the mean of the set
        /// </summary>
        /// <param name="set">A set of numbers</param>
        /// <param name="corrigated">if true return a corrigated value, otherwise normal</param>
        public static double Deviation(Set set, bool corrigated = false)
        {
            double temp = 0.0;
            double atlag = Average(set);

            Parallel.ForEach(set, itm =>
            {
                temp += Math.Pow(itm - atlag, 2);
            });

            if (corrigated) return temp / (set.Count - 1);

            return temp / set.Count;
        }

        /// <summary>
        /// returns the item that occurs most frequently in the set
        /// </summary>
        /// <param name="set">A set of numbers</param>>
        public static double Modus(Set set)
        {
            Dictionary<double, long> d = new Dictionary<double, long>();
            double maxkey = 0.0;
            long maxval = 0;

            //foreach (var val in set)
            Parallel.ForEach(set, val =>
            {
                if (d.ContainsKey((double)(val))) ++d[(double)(val)];
                else d.Add((double)(val), 1);
            });
            foreach (var key in d.Keys)
            {
                maxkey = key;
                maxval = d[key];
                break;
            }
            //foreach (var key in d.Keys)
            Parallel.ForEach(d.Keys, key =>
            {
                if (d[key] > maxval)
                {
                    maxval = d[key];
                    maxkey = key;
                }
            });
            return maxkey;
        }

        /// <summary>
        /// Returns the midle element in the sorted set
        /// </summary>
        /// <param name="set">A set of numbers</param>>
        public static double Median(Set set)
        {
            Set Copy = set.Clone();
            Copy.Sort();
            int index = Copy.Count / 2 - 1;
            if (Copy.Count % 2 == 0) return ((double)(Copy[index]) + (double)(Copy[index + 1])) / 2.0;
            return (double)(Copy[index + 1]);
        }

        /// <summary>
        /// Mean and expected value are used synonymously to refer to one measure of the central tendency either of a probability distribution or of the random variable characterized by that distribution
        /// </summary>
        /// <param name="set">A set of numbers</param>
        public static double Mean(Set set)
        {
            double mean = 0;
            int m = 0;
            //foreach (var item in set) mean += ((double)(item) - mean) / ++m;
            Parallel.ForEach(set, item =>
            {
                mean += ((double)(item) - mean) / ++m;
            });
            return mean;
        }

        /// <summary>
        /// returns how far a set of numbers is spread out
        /// </summary>
        /// <param name="set">A set of numbers</param>
        public static double Variance(Set set)
        {
            double variance = 0;
            double diff;
            double t = (double)(set[0]);
            int j = 1;
            //for (int i = 1; i < set.Count; i++)
            Parallel.For(1, set.Count, i=>
            {
                j++;
                t += (double)(set[i]);
                diff = j * (double)(set[i]) - t;
                variance += (diff * diff) / (j * (j - 1));
            });
            return variance / (j - 1);
        }
    }
}
