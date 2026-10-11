using System;
using NUnit.Framework;
using OpenQA.Selenium.Appium.Android.Enums;

namespace Appium.Net.Integration.Tests.Android
{
    public class AndroidKeyCodeTest
    {
        [Test]
        public void KeycodeMax_IsMarkedAsObsolete()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var fieldInfo = typeof(AndroidKeyCode).GetField(nameof(AndroidKeyCode.Keycode_MAX));
#pragma warning restore CS0618
            Assert.That(fieldInfo, Is.Not.Null);

            var obsoleteAttribute = Attribute.GetCustomAttribute(fieldInfo, typeof(ObsoleteAttribute)) as ObsoleteAttribute;
            Assert.That(obsoleteAttribute, Is.Not.Null);
            Assert.That(obsoleteAttribute.Message, Is.EqualTo("This constant was deprecated in API level 3. There are now more than MAX_KEYCODE keycodes. Use getMaxKeyCode() instead."));
        }
    }
}
