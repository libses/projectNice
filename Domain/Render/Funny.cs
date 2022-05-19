using System.Drawing;
using System.Numerics;

namespace Domain.Render
{
    public record FunnySettings(List<double[]> fft);
    public class Funny : Renderable<Funny, FunnySettings>
    {
        bool isGenerated = false;
        int i = 0;
        Vector2 UR;
        Vector2 UL;
        Vector2 DL;
        Vector2 DR;
        (double, double) URMinMax;
        (double, double) ULMinMax;
        (double, double) DLMinMax;
        (double, double) DRMinMax;
        (double, double)[] minMaxes;

        public Funny(int width, int height) : base(width, height)
        {
        }

        public override DirectBitmap GetBitmap()
        {
            var n = Settings.fft[0].Length;
            var discrete = Settings.fft[0].Length / n;
            if (!isGenerated)
            {
                minMaxes = new (double, double)[n];
                for (int j = 0; j < n; j++)
                {
                    minMaxes[j] = FindMinMaxOnFft(Settings.fft, j * discrete, (j + 1) * discrete);
                }

                isGenerated = true;
            }

            var r = Math.Min(Width, Height) / 2;
            var freqs = new (int, double)[n];
            for (int j = 0; j < n; j++)
            {
                freqs[j] = FindMaxAndFreq(Settings.fft[i], j * discrete, (j + 1) * discrete);
            }

            var angles = new double[n];
            for (int j = 0; j < n; j++)
            {
                angles[j] = 2 * Math.PI * freqs[j].Item1 / (discrete * n);
            }

            var rads = new double[n];
            for (int j = 0; j < n; j++)
            {
                rads[j] = r * (freqs[j].Item2 - minMaxes[j].Item1) / minMaxes[j].Item2;
            }

            var vectors = new Vector2[n];
            var fullVectors = new Vector2[n];
            for (int j = 0; j < n; j++)
            {
                vectors[j] = new Vector2(Width / 2, Height / 2) + (float)rads[j] * new Vector2((float)Math.Cos(angles[j]), (float)Math.Sin(angles[j]));
                fullVectors[j] = new Vector2(Width / 2, Height / 2) + r * new Vector2((float)Math.Cos(angles[j]), (float)Math.Sin(angles[j]));
            }

            var bmp = new DirectBitmap(Width, Height);
            var g = Graphics.FromImage(bmp.Bitmap);
            g.FillClosedCurve(Brushes.Gray, vectors.Select(x => new PointF(x.X, x.Y)).ToArray());
            foreach (var v in fullVectors)
            {
                g.DrawLine(new Pen(Color.White, 0.5f), v.X, v.Y, Width / 2, Height / 2);
            }

            foreach (var v in vectors)
            {
                g.DrawLine(new Pen(Color.White, 6), v.X, v.Y, Width / 2, Height / 2);
            }

            

            i++;
            return bmp;
        }

        private (int, double) FindMaxAndFreq(double[] fft, int down, int up)
        {
            var max = -1d;
            var maxI = -1;
            for (int i = down; i < up; i++)
            {
                if (fft[i] > max)
                {
                    max = fft[i];
                    maxI = i;
                }
            }

            return (maxI, max);
        }

        private (double, double) FindMinMaxOnFft(List<double[]> fft, int down, int up)
        {
            var max = -1d;
            var min = 10000000000000000d;

            foreach (var e in fft)
            {
                for (int i = down; i < up; i++)
                {
                    if (e[i] > max)
                    {
                        max = e[i];
                    }

                    if (e[i] < min)
                    {
                        min = e[i];
                    }
                }
            }

            return (min, max);
        }
    }
}
