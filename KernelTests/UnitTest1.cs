using FluentAssertions;
using Kernel;
using Kernel.Domain;
using Kernel.Services;

namespace KernelTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test2()
    {
        var k = new Pixel(1, 1, 1, 1);
        k.Should().Be(new Pixel(1, 1, 1, 1));
    }
}