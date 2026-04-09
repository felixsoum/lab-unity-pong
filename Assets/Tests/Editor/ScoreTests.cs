using NUnit.Framework;
using GameSystemsCookbook.Demos.PaddleBall;

namespace PaddleBall.Tests
{
    public class ScoreTests
    {
        [Test]
        public void IncrementScore_IncreasesValueByOne()
        {
            var score = new Score();

            score.IncrementScore();

            Assert.AreEqual(1, score.Value);
        }

        [Test]
        public void IncrementScore_CalledMultipleTimes_Accumulates()
        {
            var score = new Score();

            score.IncrementScore();
            score.IncrementScore();
            score.IncrementScore();

            Assert.AreEqual(3, score.Value);
        }

        [Test]
        public void ResetScore_AfterIncrements_ResetsToZero()
        {
            var score = new Score();
            score.IncrementScore();
            score.IncrementScore();

            score.ResetScore();

            Assert.AreEqual(0, score.Value);
        }

        [Test]
        public void NewScore_ValueIsZero()
        {
            var score = new Score();

            Assert.AreEqual(0, score.Value);
        }
    }
}
