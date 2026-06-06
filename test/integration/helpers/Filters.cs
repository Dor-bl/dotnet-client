using System.Collections.Generic;
using OpenQA.Selenium;

namespace Appium.Net.Integration.Tests.helpers
{
    public class Filters
    {
        public static IWebElement FirstWithName<TW>(IList<TW> els, string name) where TW : IWebElement
        {
            int count = els.Count;
            for (var i = 0; i < count; i++)
            {
                var el = els[i];
                if (el.GetAttribute("name") == name)
                {
                    return el;
                }
            }
            return null;
        }

        public static IList<IWebElement> FilterWithName<TW>(IList<TW> els, string name) where TW : IWebElement
        {
            var res = new List<IWebElement>();
            int count = els.Count;
            for (var i = 0; i < count; i++)
            {
                var el = els[i];
                if (el.GetAttribute("name") == name)
                {
                    res.Add(el);
                }
            }
            return res;
        }

        public static IList<IWebElement> FilterDisplayed<TW>(IList<TW> els) where TW : IWebElement
        {
            var res = new List<IWebElement>();
            int count = els.Count;
            for (var i = 0; i < count; i++)
            {
                IWebElement el = els[i];
                if (el.Displayed)
                {
                    res.Add(el);
                }
            }
            return res;
        }
    }
}