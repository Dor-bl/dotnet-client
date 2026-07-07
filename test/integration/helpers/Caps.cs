using System;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Enums;

namespace Appium.Net.Integration.Tests.helpers
{
    public class Caps
    {
        public static AppiumOptions GetIosCaps(string app)
        {
            var capabilities = new AppiumOptions();
            capabilities.AutomationName = AutomationName.iOSXcuiTest;
            capabilities.DeviceName = Environment.GetEnvironmentVariable("IOS_DEVICE_NAME") ?? "iPhone 17";
            capabilities.PlatformVersion =  Environment.GetEnvironmentVariable("IOS_VERSION") ?? "26.0";
            capabilities.App = app;
            capabilities.AddAdditionalAppiumOption(IOSMobileCapabilityType.LaunchTimeout, Env.InitTimeoutSec.TotalMilliseconds);

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("LOCAL_PREBUILT_WDA")))
            {
                capabilities.AddAdditionalAppiumOption("usePreinstalledWDA", true);
                capabilities.AddAdditionalAppiumOption("prebuiltWDAPath", Environment.GetEnvironmentVariable("LOCAL_PREBUILT_WDA"));
            }

            return capabilities;
        }

        public static AppiumOptions GetAndroidUIAutomatorCaps(string app)
        {
            var capabilities = new AppiumOptions();
            capabilities.AutomationName = AutomationName.AndroidUIAutomator2;
            capabilities.DeviceName = "Android Emulator";
            capabilities.App = app;
            capabilities.AddAdditionalAppiumOption("appium:adbExecTimeout", 120000);
            capabilities.AddAdditionalAppiumOption("appium:uiautomator2ServerLaunchTimeout", 120000);
            return capabilities;
        }

        public static AppiumOptions GetAndroidUIAutomatorCaps()
        {
            var capabilities = new AppiumOptions();
            capabilities.AutomationName = AutomationName.AndroidUIAutomator2;
            capabilities.DeviceName = "Android Emulator";
            capabilities.AddAdditionalAppiumOption("appium:adbExecTimeout", 120000);
            capabilities.AddAdditionalAppiumOption("appium:uiautomator2ServerLaunchTimeout", 120000);
            return capabilities;
        }

        public static AppiumOptions GetAndroidEspressoCaps(string app)
        {
            var capabilities = new AppiumOptions();
            capabilities.AutomationName = AutomationName.AndroidEspresso;
            capabilities.DeviceName = "Android Emulator";
            capabilities.App = app;
            capabilities.AddAdditionalAppiumOption("appium:adbExecTimeout", 120000);
            capabilities.AddAdditionalAppiumOption("enforceAppInstall", true);
            return capabilities;
        }
    }
}