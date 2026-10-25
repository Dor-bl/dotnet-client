using System.Collections.Generic;
using NUnit.Framework;
using OpenQA.Selenium.Appium.ImageComparison;

namespace Appium.Net.Integration.Tests.ImageComparison
{
    [TestFixture]
    public class SimilarityMatchingResultTests
    {
        [Test]
        public void Score_WithDoubleValue_ReturnsDouble()
        {
            var dict = new Dictionary<string, object>
            {
                { "score", 0.85d }
            };
            var result = new SimilarityMatchingResult(dict);
            Assert.That(result.Score, Is.EqualTo(0.85d).Within(1e-6));
        }

        [Test]
        public void Score_WithFloatValue_ReturnsDouble()
        {
            var dict = new Dictionary<string, object>
            {
                { "score", 0.75f }
            };
            var result = new SimilarityMatchingResult(dict);
            Assert.That(result.Score, Is.EqualTo(0.75d).Within(1e-6));
        }

        [Test]
        public void Score_WithIntValue_ReturnsDouble()
        {
            var dict = new Dictionary<string, object>
            {
                { "score", 1 }
            };
            var result = new SimilarityMatchingResult(dict);
            Assert.That(result.Score, Is.EqualTo(1.0d));
        }

        [Test]
        public void Score_WithLongValue_ReturnsDouble()
        {
            var dict = new Dictionary<string, object>
            {
                { "score", 0L }
            };
            var result = new SimilarityMatchingResult(dict);
            Assert.That(result.Score, Is.EqualTo(0.0d));
        }

        [Test]
        public void Score_WithStringValue_ReturnsDouble()
        {
            var dict = new Dictionary<string, object>
            {
                { "score", "0.92" }
            };
            var result = new SimilarityMatchingResult(dict);
            Assert.That(result.Score, Is.EqualTo(0.92d).Within(1e-6));
        }

        [Test]
        public void Score_WithNullValue_ReturnsZero()
        {
            var dict = new Dictionary<string, object>
            {
                { "score", null }
            };
            var result = new SimilarityMatchingResult(dict);
            Assert.That(result.Score, Is.EqualTo(0.0d));
        }

        [Test]
        public void Score_WithMissingKey_ReturnsZero()
        {
            var dict = new Dictionary<string, object>();
            var result = new SimilarityMatchingResult(dict);
            Assert.That(result.Score, Is.EqualTo(0.0d));
        }

        [Test]
        public void Score_WithNullResultDictionary_ReturnsZero()
        {
            var result = new SimilarityMatchingResult(null);
            Assert.That(result.Score, Is.EqualTo(0.0d));
        }

        [Test]
        public void Score_WithInvalidString_ReturnsZero()
        {
            var dict = new Dictionary<string, object>
            {
                { "score", "not_a_number" }
            };
            var result = new SimilarityMatchingResult(dict);
            Assert.That(result.Score, Is.EqualTo(0.0d));
        }

        [Test]
        public void Score_WithExponentialFormatString_ReturnsDouble()
        {
            var dict = new Dictionary<string, object>
            {
                { "score", "1e-2" }
            };
            var result = new SimilarityMatchingResult(dict);
            Assert.That(result.Score, Is.EqualTo(0.01d).Within(1e-6));
        }
    }
}
