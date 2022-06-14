using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kernel.Utils
{
    public static class FftHelper
    {
        public static (double, double) FindMinMax(this IEnumerable<double[]> fft, int startIndex, int endIndex)
        {
            var max = -1d;
            var min = 10000000000000000d;

            foreach (var e in fft)
            {
                for (int i = startIndex; i < endIndex; i++)
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

        public static (int, double) FindMaxFreqAndVolume(this double[] fftSlice, int startIndex, int endIndex)
        {
            var max = -1d;
            var maxI = -1;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (fftSlice[i] > max)
                {
                    max = fftSlice[i];
                    maxI = i;
                }
            }

            return (maxI, max);
        }

        public static (double, double)[] GetMinMaxes(this IEnumerable<double[]> fft, int n, int discrete)
        {
            var minMaxes = new (double, double)[n];
            for (int j = 0; j < n; j++)
            {
                minMaxes[j] = fft.FindMinMax(j * discrete, (j + 1) * discrete);
            }

            return minMaxes;
        }
    }
}
