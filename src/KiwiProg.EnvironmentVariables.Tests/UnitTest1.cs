using FluentAssertions;
using Xunit;

namespace KiwiProg.EnvironmentVariables.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var obj = new Class1();

            obj.Should().NotBeNull();
        }
    }
}
