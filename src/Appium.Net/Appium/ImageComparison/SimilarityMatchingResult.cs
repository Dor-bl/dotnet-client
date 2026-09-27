using System;
using System.Collections.Generic;
using System.Globalization;

namespace OpenQA.Selenium.Appium.ImageComparison
{
    public class SimilarityMatchingResult : ComparisonResult
    {
        /// <summary>
        /// The similarity score as a float number in range [0.0, 1.0].
        /// 1.0 is the highest score (means both images are totally equal).
        /// </summary>
        public double Score => Convert.ToDouble(Result["score"], CultureInfo.InvariantCulture);

        public SimilarityMatchingResult(Dictionary<string, object> result) : base(result)
        {
        }
    }
}
