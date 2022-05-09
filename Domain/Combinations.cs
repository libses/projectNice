﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public static class Combinations
    {
        public static IRenderable Add(this IRenderable f, IRenderable s)
        {
            return new AnonymousRender(f.GetBitmap().Add(s.GetBitmap()));
        }

        public static DirectBitmap Add(this DirectBitmap f, DirectBitmap s)
        {
            var bmp = new DirectBitmap(f.Width, f.Height);
            for (int x = 0; x < f.Width; x++)
            {
                for (int y = 0; y < f.Height; y++)
                {
                    bmp.SetPixel(x, y, f.GetPixel(x, y).Add(s.GetPixel(x, y)));
                }
            }

            return bmp;
        }

        public static IRenderable Multiply(this IRenderable f, IRenderable s)
        {
            return new AnonymousRender(f.GetBitmap().Multiply(s.GetBitmap()));
        }

        public static DirectBitmap Multiply(this DirectBitmap f, DirectBitmap s)
        {
            var bmp = new DirectBitmap(f.Width, f.Height);
            for (int x = 0; x < f.Width; x++)
            {
                for (int y = 0; y < f.Height; y++)
                {
                    bmp.SetPixel(x, y, f.GetPixel(x, y).Multiply(s.GetPixel(x, y)));
                }
            }

            return bmp;
        }

        public static Color Add(this Color f, Color s)
        {
            return Color.FromArgb((f.R + s.R).CropChannel(), (f.G + s.G).CropChannel(), (f.B + s.B).CropChannel());
        }

        public static Color Multiply(this Color f, Color s)
        {
            var R = (f.R.ToDouble() * s.R.ToDouble()).ToInt();
            var G = (f.G.ToDouble() * s.G.ToDouble()).ToInt();
            var B = (f.B.ToDouble() * s.B.ToDouble()).ToInt();
            return Color.FromArgb(R, G, B);
        }

        public static double ToDouble(this int color)
        {
            return color / 255d;
        }

        public static double ToDouble(this byte color)
        {
            return color / 255d;
        }

        public static int ToInt(this double color)
        {
            return ((int)(color * 255)).CropChannel();
        }

        public static int CropChannel(this int color)
        {
            if (color < 0) return 0;
            if (color > 255) return 255;
            return color;
        }
    }
}
