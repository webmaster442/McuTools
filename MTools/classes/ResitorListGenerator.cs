using System.Collections.Generic;

namespace MTools.classes
{
    static class ResitorListGenerator
    {
        public enum Series
        {
            e12 = 0, e24 = 1, e48 = 2, e96 = 3, e192 = 4
        }

        private static double[] multipliers = { 0.01, 0.1, 1, 10, 100, 1000, 10000 };

        private static ushort[] e12 = { 100, 120, 150, 180, 220, 270, 330, 390, 470, 560, 680, 820 };

        private static ushort[] e24 = { 100, 120, 150, 180, 220, 270, 330, 390, 470, 560, 680, 820, 110, 130, 160, 200, 240, 300, 360, 430, 510, 620, 750, 910 };

        private static ushort[] e48 = { 100, 121, 147, 178, 215, 261, 316, 383, 464, 562, 681, 825, 105, 127, 154, 187, 226, 274, 332, 
                         402, 487, 590, 715, 866, 110, 133, 162, 196, 237, 287, 348, 422, 511, 619, 750, 909, 115, 140,
                         169, 205, 249, 301, 365, 442, 536, 649, 787, 953 };

        private static ushort[] e96 = { 100, 121, 147, 178, 215, 261, 316, 383, 464, 562, 681, 825, 102, 124, 150, 182, 221, 267, 324,
                         392, 475, 576, 698, 845, 105, 127, 154, 187, 226, 274, 332, 402, 487, 590, 715, 866, 107, 130,
                         158, 191, 232, 280, 340, 412, 499, 604, 732, 887, 110, 133, 162, 196, 237, 287, 348, 422, 511,
                         619, 750, 909, 113, 137, 165, 200, 243, 294, 357, 432, 523, 634, 768, 931, 115, 140, 169, 205,
                         249, 301, 365, 442, 536, 649, 787, 953, 118, 143, 174, 210, 255, 309, 374, 453, 549, 665, 806, 976 };

        private static ushort[] e192 = { 100, 121, 147, 178, 215, 261, 316, 383, 464, 562, 681, 825, 101, 123, 149, 180, 218, 264, 320,
                          388, 470, 569, 690, 835, 102, 124, 150, 182, 221, 267, 324, 392, 475, 576, 698, 845, 104, 126,
                          152, 184, 223, 271, 328, 397, 481, 583, 706, 856, 105, 127, 154, 187, 226, 274, 332, 402, 487,
                          590, 715, 866, 106, 129, 156, 189, 229, 277, 336, 407, 493, 597, 723, 876, 107, 130, 158, 191,
                          232, 280, 340, 412, 499, 604, 732, 887, 109, 132, 160, 193, 234, 284, 344, 417, 505, 612, 741,
                          898, 110, 133, 162, 196, 237, 287, 348, 422, 511, 619, 750, 909, 111, 135, 164, 198, 240, 291,
                          352, 427, 517, 626, 759, 920, 113, 137, 165, 200, 243, 294, 357, 432, 523, 634, 768, 931, 114,
                          138, 167, 203, 246, 298, 361, 437, 530, 642, 777, 942, 115, 140, 169, 205, 249, 301, 365, 442,
                          536, 649, 787, 953, 117, 142, 172, 208, 252, 305, 370, 448, 542, 657, 796, 965, 118, 143, 174,
                          210, 255, 309, 374, 453, 549, 665, 806, 976, 120, 145, 176, 213, 258, 312, 379, 459, 556, 673, 816, 988 };

        public static List<double> GenerateList(Series serie)
        {
            List<double> ret = null;
            switch (serie)
            {
                case Series.e12:
                    ret = new List<double>(multipliers.Length * e12.Length);
                    foreach (var mul in multipliers)
                    {
                        foreach (var value in e12) ret.Add(value * mul);
                    }
                    break;
                case Series.e24:
                    ret = new List<double>(multipliers.Length * e24.Length);
                    foreach (var mul in multipliers)
                    {
                        foreach (var value in e24) ret.Add(value * mul);
                    }
                    break;
                case Series.e48:
                    ret = new List<double>(multipliers.Length * e48.Length);
                    foreach (var mul in multipliers)
                    {
                        foreach (var value in e48) ret.Add(value * mul);
                    }
                    break;
                case Series.e96:
                    ret = new List<double>(multipliers.Length * e96.Length);
                    foreach (var mul in multipliers)
                    {
                        foreach (var value in e96) ret.Add(value * mul);
                    }
                    break;
                case Series.e192:
                    ret = new List<double>(multipliers.Length * e192.Length);
                    foreach (var mul in multipliers)
                    {
                        foreach (var value in e192) ret.Add(value * mul);
                    }
                    break;
            }
            return ret;
        }

        public static List<double> GenerateList(Series serie, double mul)
        {
            List<double> ret = null;
            switch (serie)
            {
                case Series.e12:
                    ret = new List<double>(e12.Length);
                    foreach (var value in e12) ret.Add(value * mul);
                    break;
                case Series.e24:
                    ret = new List<double>(e24.Length);
                    foreach (var value in e24) ret.Add(value * mul);
                    break;
                case Series.e48:
                    ret = new List<double>(e48.Length);
                    foreach (var value in e48) ret.Add(value * mul);
                    break;
                case Series.e96:
                    ret = new List<double>(e96.Length);
                    foreach (var value in e96) ret.Add(value * mul);
                    break;
                case Series.e192:
                    ret = new List<double>(e192.Length);
                    foreach (var value in e192) ret.Add(value * mul);
                    break;
            }
            return ret;
        }

        public static double GetTolerance(Series serie)
        {
            switch (serie)
            {
                case Series.e12:
                    return 0.1;
                case Series.e24:
                    return 0.05;
                case Series.e48:
                    return 0.02;
                case Series.e96:
                    return 0.01;
                case Series.e192:
                    return 0.005;
                default:
                    return 0.1;
            }
        }
    }
}
