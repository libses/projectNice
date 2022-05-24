using System.Drawing;
using System.Numerics;
using Kernel.Domain.Utils;

namespace Kernel.Domain
{
    public struct ThreeDSettings
    {
        public readonly IReadOnlyList<double[]> FFT;

        public ThreeDSettings(List<double[]> fft)
        {
            FFT = fft;
        }
    }

    public class ThreeD : CpuRenderable<ThreeD, ThreeDSettings>
    {
        private int i = 0;
        private Vector3 sphere;
        private bool isGenerated = false;
        (double, double)[] minMaxes;

        public ThreeD(int width, int height) : base(width, height)
        {
        }

        protected override DirectBitmap Process(DirectBitmap bmp)
        {
            var n = 3;
            var discrete = Settings.FFT[0].Length / n;
            if (!isGenerated)
            {
                minMaxes = new (double, double)[n];
                for (int j = 0; j < n; j++)
                {
                    minMaxes[j] = FindMinMaxOnFft(Settings.FFT, j * discrete, (j + 1) * discrete);
                }

                isGenerated = true;
            }

            var louds = new double[n];
            for (int j = 0; j < n; j++)
            {
                louds[j] =
                    (FindMaxAndFreq(Settings.FFT[i], discrete * j, discrete * (j + 1)).Item2 - minMaxes[j].Item1) /
                    minMaxes[j].Item2;
            }

            var r = Math.Min(Width, Height);
            sphere = new Vector3((float) louds[2] * r, (float) louds[0] * r, (float) louds[1] * r); // 2white 3 9gray
            var sphereList = new List<Vector3>();
            for (double phi = -Math.PI / 2; phi <= Math.PI / 2; phi += 0.12)
            {
                for (double sigma = 0; sigma < Math.PI * 2; sigma += 0.12)
                {
                    var x = sphere.X * Math.Cos(phi + 0.785) * Math.Cos(sigma + 0.785) + r / 2;
                    var y = sphere.Y * Math.Cos(phi + Math.PI * i / 400) * Math.Sin(sigma + Math.PI * i / 400) + r / 2;
                    var z = sphere.Z * Math.Sin(phi - Math.PI * i / 1200);
                    sphereList.Add(new Vector3((float) x, (float) y, (float) z));
                }
            }

            var g = Graphics.FromImage(bmp.Bitmap);
            foreach (var point in sphereList)
            {
                var viewer = 2000;
                var distance =
                    viewer + point
                        .Z; //r dependend. r == 500 => [500, 1500], so squared [250000, 2250000]. minPoint we should have.
                var minDistance = (viewer - r) * (viewer - r);
                var minPoint = 1;
                var size = minPoint * distance * distance / minDistance;
                g.FillEllipse(Brushes.White, point.X - size / 2, point.Y - size / 2, size, size);
            }

            i++;
            return bmp;
        }

        private (double, double) FindMinMaxOnFft(IEnumerable<double[]> fft, int down, int up)
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
    }
}