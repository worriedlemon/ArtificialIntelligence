using ArtificialIntelligenceIHW.Problem;

namespace ArtificialIntelligenceIHW
{
    using Gradient = List<Color>;

    public static class Utility
    {
        public static string AddSpaces(string text)
        {
            System.Text.StringBuilder newText = new(text.Length + text.Count(char.IsUpper) - 1);
            for (int i = 0; i < text.Length; i++)
            {
                newText.Append(i != 0 && char.IsUpper(text[i]) ? $" {text[i]}" : text[i]);
            }
            return newText.ToString();
        }

        public static Color HueToRgb(double h)
        {
            int t = Convert.ToInt32(255 * Math.IEEERemainder(h / 60.0, 1.0));
            if (t < 0) t += 255;

            return (Convert.ToInt32(Math.Floor(h / 60)) % 6) switch
            {
                0 => Color.FromArgb(255, 255, t, 0),
                1 => Color.FromArgb(255, 255 - t, 255, 0),
                2 => Color.FromArgb(255, 0, 255, t),
                3 => Color.FromArgb(255, 0, 255 - t, 255),
                4 => Color.FromArgb(255, t, 0, 255),
                _ => Color.FromArgb(255, 255, 0, 255 - t)
            };
        }

        public static List<double> ContrastHues(int count)
        {
            List<double> hues = [0, 120, 240];
            double step = 120;
            double c = 3;
            double i = 0;

            double last = 60;
            while (hues.Count < count)
            {
                if (i == c)
                {
                    c *= 2;
                    i = 0;
                    last /= 2;
                    step /= 2;
                }

                hues.Add(last + step * i);
                ++i;
            }
            return hues;
        }
    }

    public static class Plotter
    {
        public static double FirstZoneEndValue = 1.25;

        public static Gradient ColorPallete = new() { Color.Red, Color.Yellow, Color.White };

        private static Color Interpolate(Color c1, Color c2, double t)
        {
            int dr = c2.R > c1.R ? c2.R - c1.R : c1.R - c2.R;
            int dg = c2.G > c1.G ? c2.G - c1.G : c1.G - c2.G;
            int db = c2.B > c1.B ? c2.B - c1.B : c1.B - c2.B;

            int ir = c2.R > c1.R ? (int)(c1.R + dr * t) : (int)(c2.R + dr * (1 - t));
            int ig = c2.G > c1.G ? (int)(c1.G + dg * t) : (int)(c2.G + dg * (1 - t));
            int ib = c2.B > c1.B ? (int)(c1.B + db * t) : (int)(c2.B + db * (1 - t));

            return Color.FromArgb(ir, ig, ib);
        }

        public static Image DrawFunction(Function f, uint segments = 50)
        {
            double[,] values = new double[segments, segments];
            double minValue = double.PositiveInfinity, maxValue = double.NegativeInfinity;

            for (int i = 0; i < segments; i++)
            {
                double tempX = f.Bounds.X + f.Bounds.Width * i / (segments - 1);
                for (int j = 0; j < segments; j++)
                {
                    double tempY = f.Bounds.Y + f.Bounds.Height * j / (segments - 1);
                    values[i, j] = f.Evaluate(tempX, tempY);

                    if (values[i, j] > maxValue) maxValue = values[i, j];
                    if (values[i, j] < minValue) minValue = values[i, j];
                }
            }

            double[] zones = new double[ColorPallete.Count];
            zones[0] = minValue;
            zones[1] = minValue + FirstZoneEndValue;
            for (int i = 2; i < zones.Length; i++)
            {
                zones[i] = zones[1] + (maxValue - zones[1]) * (i - 1) / (zones.Length - 2);
            }

            Image img = new Bitmap((int)segments, (int)segments);
            Graphics g = Graphics.FromImage(img);
            //g.ScaleTransform(1, -1);
            g.Clear(Color.White);

            for (int i = 0; i < segments; i++)
            {
                for (int j = 0; j < segments; j++)
                {
                    int k;
                    for (k = 1; k < zones.Length; k++)
                    {
                        if (zones[k] >= values[i, j]) break;
                    }
                    if (k == zones.Length) k--;

                    Color cell = Interpolate(ColorPallete[k - 1], ColorPallete[k], (values[i, j] - zones[k - 1]) / (zones[k] - zones[k - 1]));
                    g.FillRectangle(new SolidBrush(cell), new Rectangle(i, j, 1, 1));
                }
            }

            return img;
        }

        public static void DrawPoint(in Graphics g, PointF p, RectangleF offset, Size imageSize, int size = 20, int color = ~0)
        {
            PointF realPos = new(
                (p.X - offset.X) * imageSize.Width / offset.Width,
                (p.Y - offset.Y) * imageSize.Height / offset.Height
                );
            Color c = Color.FromArgb(color);
            g.FillEllipse(new SolidBrush(c), (int)realPos.X - size / 2, (int)realPos.Y - size / 2, size, size);
            g.DrawEllipse(new Pen(Color.Black), (int)realPos.X - size / 2, (int)realPos.Y - size / 2, size, size);
        }
    }
}
