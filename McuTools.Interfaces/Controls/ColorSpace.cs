// Based on code from Page Brooks, Website: http://www.pagebrooks.com, RSS Feed: http://feeds.pagebrooks.com/pagebrooks
// Modified by nokola (http://nokola.com) to include rgv to hsv color space translation

using System;
using System.Windows.Media;

namespace McuTools.Interfaces.Controls
{
    internal static class ColorSpace
    {
        private const byte MIN = 0;
        private const byte MAX = 255;

        public static Color GetColorFromPosition(int position)
        {
            byte mod = (byte)(position % MAX);
            byte diff = (byte)(MAX - mod);
            byte alpha = 255;

            switch (position / MAX)
            {
                case 0: return Color.FromArgb(alpha, MAX, mod, MIN);
                case 1: return Color.FromArgb(alpha, diff, MAX, MIN);
                case 2: return Color.FromArgb(alpha, MIN, MAX, mod);
                case 3: return Color.FromArgb(alpha, MIN, diff, MAX);
                case 4: return Color.FromArgb(alpha, mod, MIN, MAX);
                case 5: return Color.FromArgb(alpha, MAX, MIN, diff);
                default: return Colors.Black;
            }
        }

        public static string GetHexCode(Color c)
        {
            return string.Format("#{0}{1}{2}{3}",
                c.A.ToString("X2"),
                c.R.ToString("X2"),
                c.G.ToString("X2"),
                c.B.ToString("X2"));
        }

        public static void ConvertRgbToHsv(double r, double g, double b, out double h, out double s, out double v)
        {
            double colorMax = Math.Max(Math.Max(r, g), b);

            v = colorMax;
            if (v == 0)
            {
                h = 0;
                s = 0;
                return;
            }

            // normalize
            r /= v;
            g /= v;
            b /= v;

            double colorMin = Math.Min(Math.Min(r, g), b);
            colorMax = Math.Max(Math.Max(r, g), b);

            s = colorMax - colorMin;
            if (s == 0)
            {
                h = 0;
                return;
            }

            // normalize saturation
            r = (r - colorMin) / s;
            g = (g - colorMin) / s;
            b = (b - colorMin) / s;
            colorMax = Math.Max(Math.Max(r, g), b);

            // calculate hue
            if (colorMax == r)
            {
                h = 0.0 + 60.0 * (g - b);
                if (h < 0.0)
                {
                    h += 360.0;
                }
            }
            else if (colorMax == g)
            {
                h = 120.0 + 60.0 * (b - r);
            }
            else // colorMax == b
            {
                h = 240.0 + 60.0 * (r - g);
            }

        }

        // Algorithm ported from: http://www.colorjack.com/software/dhtml+color+picker.html
        public static Color ConvertHsvToRgb(float h, float s, float v)
        {
            h = h / 360;
            if (s > 0)
            {
                if (h >= 1)
                    h = 0;
                h = 6 * h;
                int hueFloor = (int)Math.Floor(h);
                byte a = (byte)Math.Round(MAX * v * (1.0 - s));
                byte b = (byte)Math.Round(MAX * v * (1.0 - (s * (h - hueFloor))));
                byte c = (byte)Math.Round(MAX * v * (1.0 - (s * (1.0 - (h - hueFloor)))));
                byte d = (byte)Math.Round(MAX * v);

                switch (hueFloor)
                {
                    case 0: return Color.FromArgb(MAX, d, c, a);
                    case 1: return Color.FromArgb(MAX, b, d, a);
                    case 2: return Color.FromArgb(MAX, a, d, c);
                    case 3: return Color.FromArgb(MAX, a, b, d);
                    case 4: return Color.FromArgb(MAX, c, a, d);
                    case 5: return Color.FromArgb(MAX, d, a, b);
                    default: return Color.FromArgb(0, 0, 0, 0);
                }
            }
            else
            {
                byte d = (byte)(v * MAX);
                return Color.FromArgb(255, d, d, d);
            }
        }
    }
}

