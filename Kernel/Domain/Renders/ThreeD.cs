using System.Drawing;
using System.Numerics;
using Kernel.Domain.Utils;
using Kernel.Utils;

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
    public class ThreeD : Renderable<ThreeD, ThreeDSettings>
    {
        private int i = 0;
        private Vector3 sphere;
        private bool isGenerated = false;
        (double, double)[] minMaxes;
        private DirectBitmap bmp;
        public ThreeD(int width, int height) : base(width, height)
        {
            bmp = new DirectBitmap(width, height);
        }

        public override DirectBitmap GetBitmap()
        {
            var viewDistance = 2000; 
            for (int i = 0; i < bmp.Data.Length; i++)
            {
                bmp.Data[i] = 0;
            }

            var g = Graphics.FromImage(bmp.Bitmap);
            var n = 3;
            var discrete = Settings.FFT[0].Length / n;
            if (!isGenerated)
            {
                minMaxes = Settings.FFT.GetMinMaxes(n, discrete);
                isGenerated = true;
            }

            var louds = new double[n];
            for (int j = 0; j < n; j++)
            {
                louds[j] = (Settings.FFT[i].FindMaxFreqAndVolume(discrete * j, discrete * (j + 1)).Item2 - minMaxes[j].Item1) / minMaxes[j].Item2;
            }

            var radius = Math.Min(Width, Height);
            sphere = new Vector3((float)louds[2] * radius, (float)louds[0] * radius, (float)louds[1] * radius);
            for (double phi = -Math.PI / 2; phi <= Math.PI / 2; phi += 0.12)
            {
                for (double sigma = 0; sigma < Math.PI * 2; sigma += 0.12)
                {
                    var x = (float)(sphere.X * Math.Cos(phi + 0.785) * Math.Cos(sigma + 0.785) + radius/2);
                    var y = (float)(sphere.Y * Math.Cos(phi + Math.PI * i / 400) * Math.Sin(sigma + Math.PI * i / 400) + radius/2);
                    var z = (float)(sphere.Z * Math.Sin(phi - Math.PI * i / 1200));
                    var distance = viewDistance + z; //r dependend. r == 500 => [500, 1500], so squared [250000, 2250000]. minPoint we should have.
                    var minDistance = (viewDistance - radius) * (viewDistance - radius);
                    var minPointSize = 1;
                    var size = minPointSize * distance * distance / minDistance;
                    g.FillEllipse(Brushes.White, x - size / 2, y - size / 2, size, size);
                }
            }

            i++;
            return bmp;
        }
    }
}
