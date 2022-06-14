using System.Drawing;
using System.Numerics;
using Kernel.Domain.Settings;
using Kernel.Domain.Utils;
using Kernel.Utils;

namespace Kernel.Domain
{
    public class Funny : Renderable<Funny, FunnySettings>
    {
        bool isMinMaxesFilled = false;
        int i = 0;
        (double, double)[] minMaxes;
        private DirectBitmap bmp;

        public Funny(int width, int height) : base(width, height)
        {
            bmp = new DirectBitmap(width, height);
        }

        public override DirectBitmap GetBitmap()
        {
            for (int i = 0; i < bmp.Data.Length; i++)
            {
                bmp.Data[i] = 0;
            }

            var g = Graphics.FromImage(bmp.Bitmap);
            var n = Settings.Fft[0].Length;
            var discrete = Settings.Fft[0].Length / n;
            if (!isMinMaxesFilled)
                FillMinMaxes(n, discrete);

            var r = Math.Min(Width, Height) / 2;
            for (int j = 0; j < n; j++)
            {
                var freq = Settings.Fft[i].FindMaxFreqAndVolume(j * discrete, (j + 1) * discrete);
                var angle = 2 * Math.PI * freq.Item1 / (discrete * n);
                var radius = r * (freq.Item2 - minMaxes[j].Item1) / minMaxes[j].Item2;
                var v = new Vector2(Width / 2, Height / 2) + (float)radius * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                g.DrawLine(new Pen(Color.White, 0.5f), v.X, v.Y, Width / 2, Height / 2);
            }

            i++;
            return bmp;
        }

        private void FillMinMaxes(int n, int discrete)
        {
            minMaxes = new (double, double)[n];
            for (int j = 0; j < n; j++)
            {
                minMaxes[j] = Settings.Fft.FindMinMax(j * discrete, (j + 1) * discrete);
            }

            isMinMaxesFilled = true;
        }
    }
}
