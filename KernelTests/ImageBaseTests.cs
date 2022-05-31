using System.Drawing;
using Kernel.Domain;
using Kernel.Domain.Settings;
using Kernel.Domain.Utils;
using Kernel.Services;

namespace KernelTests;

[TestFixture]
public class ImageBaseTests
{
    [Test]
    public void TestGetBitmapAfterAdd()
    {
        var imageBase = ImageBase.Create().Config(new ImageSettings(1, 1))
            .Add<Constant>(c => c.Config(new ConstantSettings(Color.White)))
            .Add<Constant>(c => c.Config(new ConstantSettings(Color.Aqua)))
            .Add<Constant>(c => c.Config(new ConstantSettings(Color.Orange)));

        var bitmap = imageBase.GetBitmap().Bitmap;

        bitmap.Size.Should().Be(new Size(1, 1));
        bitmap.GetPixel(0, 0).Should().Be(Color.White.Add(Color.Aqua).Add(Color.Orange));
    }
    
    [Test]
    public void TestGetBitmapAfterMultiply()
    {
        var imageBase = ImageBase.Create().Config(new ImageSettings(1, 1))
            .Add<Constant>(c => c.Config(new ConstantSettings(Color.White)))
            .Multiply<Constant>(c => c.Config(new ConstantSettings(Color.Aqua)))
            .Multiply<Constant>(c => c.Config(new ConstantSettings(Color.Orange)));

        var bitmap = imageBase.GetBitmap().Bitmap;

        bitmap.Size.Should().Be(new Size(1, 1));
        bitmap.GetPixel(0, 0).Should().Be(Color.White.Multiply(Color.Aqua).Multiply(Color.Orange));
    }
    
    [Test]
    public void TestGetBitmapAfterMultiplyAndAdd()
    {
        var imageBase = ImageBase.Create().Config(new ImageSettings(1, 1))
            .Add<Constant>(c => c.Config(new ConstantSettings(Color.White)))
            .Add<Constant>(c => c.Config(new ConstantSettings(Color.Aqua)))
            .Multiply<Constant>(c => c.Config(new ConstantSettings(Color.Orange)));

        var bitmap = imageBase.GetBitmap().Bitmap;

        bitmap.Size.Should().Be(new Size(1, 1));
        bitmap.GetPixel(0, 0).Should().Be(Color.White.Add(Color.Aqua).Multiply(Color.Orange));
    }
    
    [Test]
    public void TestGetBitmapAfterAddAndMultiply()
    {
        var imageBase = ImageBase.Create().Config(new ImageSettings(1, 1))
            .Add<Constant>(c => c.Config(new ConstantSettings(Color.Brown)))
            .Multiply<Constant>(c => c.Config(new ConstantSettings(Color.Aqua)))
            .Add<Constant>(c => c.Config(new ConstantSettings(Color.Orange)));

        var bitmap = imageBase.GetBitmap().Bitmap;

        bitmap.Size.Should().Be(new Size(1, 1));
        bitmap.GetPixel(0, 0).Should().Be(Color.Brown.Multiply(Color.Aqua).Add(Color.Orange));
    }
}