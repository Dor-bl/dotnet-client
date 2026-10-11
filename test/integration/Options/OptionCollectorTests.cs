using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.Service.Options;

namespace Appium.Net.Integration.Tests.Options
{
    [TestFixture]
    public class OptionCollectorTests
    {
        [Test]
        public void ParseCapabilitiesIfWindows_ShouldEscapeQuotesCorrectly()
        {
            // Arrange
            var collector = new OptionCollector();
            var method = typeof(OptionCollector).GetMethod("ParseCapabilitiesIfWindows", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(method, Is.Not.Null, "Method 'ParseCapabilitiesIfWindows' should exist on OptionCollector.");

            var capabilities = new Dictionary<string, object>
            {
                { "normalKey", "normalValue" },
                { "maliciousKey\" : \"injected\"", "maliciousValue" },
                { "anotherKey", "value\" : \"injected" }
            };

            // Act
            var result = (string)method.Invoke(collector, new object[] { capabilities });

            // Assert
            Assert.That(result, Does.StartWith("\""));
            Assert.That(result, Does.EndWith("\""));

            // Verify that keys and values are safely represented in the output
            Assert.That(result, Does.Contain("normalKey"));
            Assert.That(result, Does.Contain("normalValue"));
            Assert.That(result, Does.Contain("maliciousKey"));
            Assert.That(result, Does.Contain("maliciousValue"));
            Assert.That(result, Does.Contain("anotherKey"));
            Assert.That(result, Does.Contain("injected"));

            // Ensure raw unescaped quotes cannot break the JSON structure or the command line argument
            Assert.That(result, Does.Not.Contain("maliciousKey\" : \"injected\""));
            Assert.That(result, Does.Not.Contain("value\" : \"injected"));

            // Validate that malicious quotes are safely escaped (either \u0022 or escaped backslash-quotes)
            Assert.That(
                result.Contains("maliciousKey\\u0022 : \\u0022injected\\u0022") ||
                result.Contains("maliciousKey\\\\\\\" : \\\\\\\"injected\\\\\\\"") ||
                result.Contains("maliciousKey\\\" : \\\"injected\\\""),
                Is.True,
                "Malicious key quotes must be safely escaped.");

            Assert.That(
                result.Contains("value\\u0022 : \\u0022injected") ||
                result.Contains("value\\\\\\\" : \\\\\\\"injected") ||
                result.Contains("value\\\" : \\\"injected"),
                Is.True,
                "Malicious value quotes must be safely escaped.");
        }

        [Test]
        public void ParseCapabilitiesIfWindows_ShouldHandleWindowsFilePaths()
        {
            // Arrange
            var collector = new OptionCollector();
            var method = typeof(OptionCollector).GetMethod("ParseCapabilitiesIfWindows", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.That(method, Is.Not.Null, "Method 'ParseCapabilitiesIfWindows' should exist on OptionCollector.");

            var capabilities = new Dictionary<string, object>
            {
                { MobileCapabilityType.App, @"C:\path\to\app.apk" }
            };

            // Act
            var result = (string)method.Invoke(collector, new object[] { capabilities });

            // Assert
            Assert.That(result, Does.Contain("C:/path/to/app.apk"));
        }

        [Test]
        public void EscapeWindowsArgument_ShouldFollowWindowsCommandLineQuotingRules()
        {
            // Arrange
            var method = typeof(OptionCollector).GetMethod("EscapeWindowsArgument", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.That(method, Is.Not.Null, "Method 'EscapeWindowsArgument' should exist on OptionCollector.");

            // Act & Assert
            // Quotes preceded by backslashes must double the backslashes plus one (2N + 1)
            // so an odd number of backslashes precedes the quote, making it a literal quote in Windows CRT
            var resultWithEscapedQuote = (string)method.Invoke(null, new object[] { "test\\\"quote" });
            Assert.That(resultWithEscapedQuote, Is.EqualTo("\"test\\\\\\\"quote\""));

            // Trailing backslashes before the closing quote must be doubled (2N) so they don't escape the closing quote
            var resultWithTrailingBackslash = (string)method.Invoke(null, new object[] { @"C:\path\dir\" });
            Assert.That(resultWithTrailingBackslash, Is.EqualTo("\"C:\\path\\dir\\\\\""));

            // Backslashes not preceding a quote should be preserved literally
            var resultWithLiteralBackslash = (string)method.Invoke(null, new object[] { @"C:\path\to\file" });
            Assert.That(resultWithLiteralBackslash, Is.EqualTo("\"C:\\path\\to\\file\""));

            // Empty or null string should produce empty quoted string
            var resultEmpty = (string)method.Invoke(null, new object[] { string.Empty });
            Assert.That(resultEmpty, Is.EqualTo("\"\""));

            var resultNull = (string)method.Invoke(null, new object[] { null });
            Assert.That(resultNull, Is.EqualTo("\"\""));
        }
    }
}
