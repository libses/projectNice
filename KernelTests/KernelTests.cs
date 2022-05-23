using Kernel;
using Kernel.Domain;
using Kernel.Domain.Settings;
using Kernel.Domain.Utils;
using Kernel.Services;
using System.Drawing;
using Kernel.Services.Interfaces;

namespace KernelTests
{
    [TestFixture]
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

    [TestFixture]
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

    [TestFixture]
    public class ConstantShould
    {
        [Test]
        public void TestConstantInitColor()
        {
            var c = new Constant(100, 100).Config(new ConstantSettings(Color.FromArgb(12, 34, 56, 78)));
            c.GetBitmap().GetPixel(1, 1).Should().Be(Color.FromArgb(12, 34, 56, 78));
        }

        [Test]
        public void TestConstantInitPixel()
        {
            var c = new Constant(100, 100).Config(new ConstantSettings(new Pixel(12, 34, 56, 78)));
            c.GetBitmap().GetPixel(1, 1).Should().Be(Color.FromArgb(78, 12, 34, 56));
        }
    }

    [TestFixture]
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
        public void TestAddOverflow()
        {
            var f = new Constant(100, 100).Config(new ConstantSettings(Color.FromArgb(200, 200, 200)));
            var s = new Constant(100, 100).Config(new ConstantSettings(Color.FromArgb(200, 200, 200)));
            var bmp = f.GetBitmap();
            bmp.Add(s.GetBitmap());
            bmp.GetPixel(0, 1).Should().Be(Color.FromArgb(255, 255, 255));
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

    [TestFixture]
    public class KernelBuilderTests
    {
        [Test]
        public void TestKernelBuildNotCrash()
        {
            Action creation = () => KernelBuilder.Create();
            creation.Should().NotThrow();
        }
    }

    [TestFixture]
    public class BitmapExtensionsTests
    {
        [Test]
        public void TestJPGSave()
        {
            var f = new Constant(100, 100).Config(new ConstantSettings(Color.FromArgb(123, 123, 123)));
            f.GetBitmap().Bitmap.SaveJPG100("aboba.jpg");
            var bmp = Bitmap.FromFile("aboba.jpg");
            ((Bitmap) bmp).GetPixel(0, 0).Should().Be(Color.FromArgb(123, 123, 123));
            bmp.Dispose();
            File.Delete("aboba.jpg");
        }
    }

    [TestFixture]
    public class BitmapProviderTests
    {
        [Test]
        public void TestProvide()
        {
            Directory.CreateDirectory("temp");
            var f = new Constant(100, 100).Config(new ConstantSettings(Color.FromArgb(123, 123, 123)));
            var bmp = f.GetBitmap().Bitmap;
            bmp.SaveJPG100("temp\\0.jpg");
            bmp.SaveJPG100("temp\\1.jpg");
            var provider = new BitmapProvider("temp", "jpg", 2);
            var bmps = provider.Get().ToList();
            bmps.Count.Should().Be(2);
            bmps[0].GetPixel(0, 0).Should().Be(bmp.GetPixel(0, 0));
            bmp.Dispose();
            provider.Dispose();
            File.Delete("temp\\0.jpg");
            File.Delete("temp\\1.jpg");
            Directory.Delete("temp");
        }
    }

    [TestFixture]
    public class FunnyTests
    {
        [Test]
        public void TestFunnyEmptyFft()
        {
            var doubles = new double[100];
            var doublesList = new List<double[]> {doubles};
            var funny = new Funny(100, 100).Config(new FunnySettings(doublesList));
            funny.GetBitmap().GetPixel(0, 0).Should().Be(Color.FromArgb(0, 0, 0, 0));
        }
    }

    [TestFixture]
    public class FFTGeneratorTests
    {
        [Test]
        public void GetFFTShouldUseIWavAudioProvider()
        {
            var filename = "aboba.wav";
            var provider = A.Fake<IWavAudioProvider>();
            var generator = new FFTGenerator(provider);

            generator.GetFFT(filename);

            A.CallTo(() => provider.ReadWav(filename)).MustHaveHappenedOnceExactly();
        }
    }
}