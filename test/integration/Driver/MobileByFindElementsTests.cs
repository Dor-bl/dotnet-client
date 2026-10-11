using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Appium.Net.Integration.Tests.helpers;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Interfaces;

namespace Appium.Net.Integration.Tests.Driver
{
    [TestFixture]
    public class MobileByFindElementsTests
    {
        /// <summary>
        /// A search context that hands back whatever collection the test supplies, so the
        /// conversion done by <see cref="MobileBy"/> can be exercised without a real driver.
        /// </summary>
        private class StubAccessibilityIdContext : IFindByAccessibilityId<IWebElement>, ISearchContext
        {
            private readonly IReadOnlyCollection<IWebElement> _elements;

            public StubAccessibilityIdContext(IReadOnlyCollection<IWebElement> elements)
            {
                _elements = elements;
            }

            public IReadOnlyCollection<IWebElement> FindElementsByAccessibilityId(string selector) => _elements;

            public IWebElement FindElementByAccessibilityId(string selector) => throw new NotImplementedException();

            public IReadOnlyCollection<IWebElement> FindElements(string selector, string value) => _elements;

            public IWebElement FindElement(string by, string value) => throw new NotImplementedException();

            public ReadOnlyCollection<IWebElement> FindElements(By by) => throw new NotImplementedException();

            public IWebElement FindElement(By by) => throw new NotImplementedException();
        }

        /// <summary>
        /// Implements only the fluent selector interface, so locators fall through to
        /// <see cref="MobileBy.FindElements(ISearchContext)"/> instead of the specialised override.
        /// </summary>
        private class StubFluentSelectorContext : IFindsByFluentSelector<IWebElement>, ISearchContext
        {
            private readonly IReadOnlyCollection<IWebElement> _elements;

            public StubFluentSelectorContext(IReadOnlyCollection<IWebElement> elements)
            {
                _elements = elements;
            }

            public IReadOnlyCollection<IWebElement> FindElements(string selector, string value) => _elements;

            public IWebElement FindElement(string by, string value) => throw new NotImplementedException();

            public ReadOnlyCollection<IWebElement> FindElements(By by) => throw new NotImplementedException();

            public IWebElement FindElement(By by) => throw new NotImplementedException();
        }

        [Test]
        public void FindElements_WhenFinderReturnsNull_ThrowsArgumentNullExceptionNamingTheCollection()
        {
            var context = new StubAccessibilityIdContext(null);

            var exception = Assert.Throws<ArgumentNullException>(
                (Action)(() => MobileBy.AccessibilityId("some-id").FindElements(context)));

            Assert.That(exception.ParamName, Is.EqualTo("collection"));
        }

        [Test]
        public void FindElements_WhenFluentSelectorFinderReturnsNull_ThrowsArgumentNullExceptionNamingTheCollection()
        {
            var context = new StubFluentSelectorContext(null);

            var exception = Assert.Throws<ArgumentNullException>(
                (Action)(() => MobileBy.AccessibilityId("some-id").FindElements(context)));

            Assert.That(exception.ParamName, Is.EqualTo("collection"));
        }

        [Test]
        public void FindElements_WhenFinderReturnsList_PreservesElementsAndOrder()
        {
            var first = new TestableAppiumElement();
            var second = new TestableAppiumElement();
            var context = new StubAccessibilityIdContext(new List<IWebElement> { first, second });

            var result = MobileBy.AccessibilityId("some-id").FindElements(context);

            Assert.That(result, Is.EqualTo(new IWebElement[] { first, second }));
        }

        [Test]
        public void FindElements_WhenFinderReturnsReadOnlyCollection_ReusesTheSameInstance()
        {
            var elements = new ReadOnlyCollection<IWebElement>(
                new List<IWebElement> { new TestableAppiumElement() });
            var context = new StubAccessibilityIdContext(elements);

            var result = MobileBy.AccessibilityId("some-id").FindElements(context);

            Assert.That(result, Is.SameAs(elements));
        }

        [Test]
        public void FindElements_WhenFinderReturnsEmptyCollection_ReturnsEmptyCollection()
        {
            var context = new StubAccessibilityIdContext(new List<IWebElement>());

            var result = MobileBy.AccessibilityId("some-id").FindElements(context);

            Assert.That(result, Is.Empty);
        }
    }
}
