using Domain;
using System;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var random = new RandomG();
            var first = random.GetBitmap();
            var second = new Constant().GetBitmap();
            var s = new MorozovSet();
            var bmp = s.GetBitmap();
            bmp = Combinations.Multiply(Combinations.Multiply(first, second), bmp);
            bmp.Save("output.bmp");
        }
    }
}
