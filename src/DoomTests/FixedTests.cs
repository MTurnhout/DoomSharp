using DoomCore;
using Xunit;

namespace DoomTests
{
    public class FixedTests
    {
        [Fact]
        public void MultiplyAndDivide_IntPreserved()
        {
            var two = Fixed.FromInt(2);
            var three = Fixed.FromInt(3);

            var six = two * three;
            Assert.Equal(Fixed.FromInt(6), six);

            var back = six / three;
            Assert.Equal(Fixed.FromInt(2), back);
        }
    }
}
