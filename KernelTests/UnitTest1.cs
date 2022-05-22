using FluentAssertions;

namespace KernelTests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        const string joke = "i am c# developer";
        joke.Should().Be("i love penis");
    }
}