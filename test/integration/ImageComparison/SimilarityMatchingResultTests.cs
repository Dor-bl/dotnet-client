using System.Collections.Generic;
using NUnit.Framework;
using OpenQA.Selenium.Appium.ImageComparison;

namespace Appium.Net.Integration.Tests.ImageComparison
{
    [TestFixture]
    public class SimilarityMatchingResultTests
    {
        [Test]
        public void Score_WithDoubleObject_ReturnsCorrectDouble()
        {
            var resultDict = new Dictionary<string, object>
            {
                { "score", 0.85d }
            };
            var similarityResult = new SimilarityMatchingResult(resultDict);

            Assert.That(similarityResult.Score, Is.EqualTo(0.85d));
        }

        [Test]
        public void Score_WithStringObject_ReturnsCorrectDouble()
        {
            var resultDict = new Dictionary<string, object>
            {
                { "score", "0.75" }
            };
            var similarityResult = new SimilarityMatchingResult(resultDict);

            Assert.That(similarityResult.Score, Is.EqualTo(0.75d));
        }

        [Test]
        public void Score_WithIntegerObject_ReturnsCorrectDouble()
        {
            var resultDict = new Dictionary<string, object>
            {
                { "score", 1 }
            };
            var similarityResult = new SimilarityMatchingResult(resultDict);

            Assert.That(similarityResult.Score, Is.EqualTo(1.0d));
        }
    }
}
