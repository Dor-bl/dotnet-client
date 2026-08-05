using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using OpenQA.Selenium.Appium.Service.Options;
using OpenQA.Selenium.Appium.Enums;

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

            var capabilities = new Dictionary<string, object>
            {
                { "normalKey", "normalValue" },
                { "maliciousKey\" : \"injected\"", "maliciousValue" },
                { "anotherKey", "value\" : \"injected" }
            };

            // Act
            var result = (string)method.Invoke(collector, new object[] { capabilities });

            // Assert
            Console.WriteLine($"Result: {result}");

            Assert.That(result, Does.StartWith("\"{"));
            Assert.That(result, Does.EndWith("}\""));

            // With JsonSerializer.Serialize, internal quotes are escaped: \"
            // Then we replace " with \": \\\"
            // So "anotherKey": "value\" : \"injected" becomes something like:
            // \\\"anotherKey\\\":\\\"value\\\\\\\" : \\\\\\\"injected\\\"

            Assert.That(result, Does.Contain("\\\"normalKey\\\":\\\"normalValue\\\""));
            Assert.That(result, Does.Contain("\\\"anotherKey\\\":\\\"value\\\\\\\" : \\\\\\\"injected\\\""));
        }

        [Test]
        public void ParseCapabilitiesIfWindows_ShouldHandleWindowsFilePaths()
        {
            // Arrange
            var collector = new OptionCollector();
            var method = typeof(OptionCollector).GetMethod("ParseCapabilitiesIfWindows", BindingFlags.NonPublic | BindingFlags.Instance);

            var capabilities = new Dictionary<string, object>
            {
                { MobileCapabilityType.App, @"C:\path\to\app.apk" }
            };

            // Act
            var result = (string)method.Invoke(collector, new object[] { capabilities });

            // Assert
            Assert.That(result, Does.Contain("C:/path/to/app.apk"));
        }
    }
}
