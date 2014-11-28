using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTools.classes
{
    static class ResistorValueSolver
    {
        public static string Solve(double desiredvalue, ResitorListGenerator.Series serie)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("--------------------------------------------------------------------");
            sb.AppendLine("Serial configuration:");
            sb.AppendLine("--------------------------------------------------------------------");
            double remain = desiredvalue;
            double sum = 0;
            var list = ResitorListGenerator.GenerateList(serie);
            int i, j;

            for (i = 0; i < 3; i++)
            {
                var q = (from item in list where item <= remain orderby item descending select item).FirstOrDefault();
                if (q != 0)
                {
                    remain -= q;
                    sum += q;
                    sb.AppendLine(q.ToString() + " Ω");
                }
            }
            sb.AppendLine("--------------------------------------------------------------------");
            sb.AppendFormat("Sum value: {0} Ω\r\n", sum);
            sb.AppendFormat("Error: {0} Ω\r\n", remain);
            sb.AppendFormat("Error(%): {0} %\r\n", Math.Round(remain / sum, 4) * 100);

            sb.Append("\r\n\r\n\r\n");

            sb.AppendLine("--------------------------------------------------------------------");
            sb.AppendLine("Parallel configuration:");
            sb.AppendLine("--------------------------------------------------------------------");

            list = (from item in ResitorListGenerator.GenerateList(serie) where item > (desiredvalue / 2) select item).ToList();
            Dictionary<double, double[]> _results = new Dictionary<double, double[]>();
            double marginp = desiredvalue + (desiredvalue * ResitorListGenerator.GetTolerance(serie));
            double marginn = desiredvalue - (desiredvalue * ResitorListGenerator.GetTolerance(serie));

            for (i = 0; i < list.Count; i++)
            {
                for (j = 0; j < list.Count; j++)
                {
                    double value = (list[i] * list[j]) / (list[i] + list[j]);
                    if ((value > marginn) && (value < marginp))
                    {
                        if (_results.ContainsKey(value)) _results[value] = new double[] { list[i], list[j] };
                        else _results.Add(value, new double[] { list[i], list[j] });
                    }
                }
            }

            var best = (from result in _results where result.Key <= desiredvalue orderby (desiredvalue - result.Key) ascending select result).FirstOrDefault();
            if (best.Value != null)
            {
                sb.AppendLine(best.Value[0].ToString() + " Ω");
                sb.AppendLine(best.Value[1].ToString() + " Ω");
            }
            sb.AppendLine("--------------------------------------------------------------------");
            sb.AppendFormat("Paralell value: {0} Ω\r\n", best.Key);
            sb.AppendFormat("Error: {0} Ω\r\n", desiredvalue - best.Key);
            sb.AppendFormat("Error(%): {0} %\r\n", Math.Round((desiredvalue - best.Key) / desiredvalue, 4) * 100);

            return sb.ToString();

        }
    }
}
