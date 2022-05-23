using FakeItEasy;
using FluentAssertions;
using Kernel.Domain;
using Kernel.Domain.Settings;
using Kernel.Domain.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KernelTests
{
    public class PlanetsShould
    {
        [Test]
        public void TestPlanetsInit()
        {
            var r = A.Fake<Random>();
            A.CallTo(() => r.Next(0, 10)).WithAnyArguments().Returns(5);
            var planets = new Planets(100, 100).Config(new PlanetsSettings(1, 0, 10, Brushes.White, r));
            var bmp = planets.GetBitmap();
            bmp.GetPixel(5, 5).Should().Be(Color.FromArgb(255, 255, 255));
            bmp.GetPixel(0, 0).Should().NotBe(Color.FromArgb(255, 255, 255));
        }

        [Test]
        public void TestPlanetsMove()
        {
            var r = A.Fake<Random>();
            A.CallTo(() => r.Next(0, 10)).WithAnyArguments().ReturnsNextFromSequence(75, 50, 10, 25, 50, 10);
            var planets = new Planets(100, 100).Config(new PlanetsSettings(2, 0, 10, Brushes.White, r));
            var bmp = planets.GetBitmap();
            bmp.GetPixel(79, 50).Should().Be(Color.FromArgb(255, 255, 255));
            bmp.GetPixel(21, 50).Should().Be(Color.FromArgb(255, 255, 255));
            for (int i = 0; i < 1000; i++)
            {
                planets.GetBitmap();
            }

            bmp = planets.GetBitmap();
            bmp.GetPixel(79, 50).Should().NotBe(Color.FromArgb(255, 255, 255));
            bmp.GetPixel(21, 50).Should().NotBe(Color.FromArgb(255, 255, 255));
        }
    }

    public class RandomShould
    {
        [Test]
        public void TestRandomInit()
        {
            var r = A.Fake<Random>();
            A.CallTo(() => r.Next(0, 0)).WithAnyArguments().Returns(100);
            var rand = new RandomG(100, 100).Config(new RandomSettings(0, 0, r));
            rand.GetBitmap().GetPixel(0, 0).Should().Be(Color.FromArgb(100, 100, 100));
        }
    }

    public class ConstantShould
    {
        [Test]
        public void TestConstantInit()
        {
            var c = new Constant(100, 100).Config(new ConstantSettings(Color.FromArgb(12, 34, 56, 78)));
            c.GetBitmap().GetPixel(1, 1).Should().Be(Color.FromArgb(12, 34, 56, 78));
        }
    }

    public class OperatorsShould
    {
        [Test]
        public void TestAdd()
        {
            var f = new Constant(100, 100).Config(new ConstantSettings(Color.FromArgb(2, 2, 2)));
            var s = new Constant(100, 100).Config(new ConstantSettings(Color.FromArgb(3, 3, 3)));
            var bmp = f.GetBitmap();
            bmp.Add(s.GetBitmap());
            bmp.GetPixel(0, 1).Should().Be(Color.FromArgb(5, 5, 5));
        }

        [Test]
        public void TestMultiply()
        {
            var f = new Constant(100, 100).Config(new ConstantSettings(Color.FromArgb(255, 255, 255)));
            var s = new Constant(100, 100).Config(new ConstantSettings(Color.FromArgb(1, 1, 1)));
            var bmp = f.GetBitmap();
            bmp.Multiply(s.GetBitmap());
            bmp.GetPixel(0, 1).Should().Be(Color.FromArgb(1, 1, 1));
        }
    }
}
