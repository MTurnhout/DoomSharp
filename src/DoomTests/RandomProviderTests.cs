using DoomCore;
using Xunit;

namespace DoomTests
{
    public class RandomProviderTests
    {
        [Fact]
        public void M_Random_Sequence_MatchesOriginal()
        {
            var r = new RandomProvider();
            r.M_ClearRandom();

            // First few values from rndtable after initial M_Random calls
            Assert.Equal(8, r.M_Random());   // rndtable[1]
            Assert.Equal(109, r.M_Random()); // rndtable[2]
            Assert.Equal(220, r.M_Random()); // rndtable[3]
            Assert.Equal(222, r.M_Random()); // rndtable[4]
        }

        [Fact]
        public void P_Random_Sequence_MatchesOriginal()
        {
            var r = new RandomProvider();
            r.M_ClearRandom();

            Assert.Equal(8, r.P_Random());
            Assert.Equal(109, r.P_Random());
        }
    }
}
