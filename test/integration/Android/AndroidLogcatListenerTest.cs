﻿using Appium.Net.Integration.Tests.helpers;
using NUnit.Framework;
using OpenQA.Selenium.Appium.Android;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Appium.Net.Integration.Tests.Android
{
    [TestFixture]
    public class AndroidLogcatListenerTest
    {
        private AndroidDriver _driver;
        private Uri serverUri;
        private readonly SemaphoreSlim messageSemaphore = new SemaphoreSlim(1);
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(40);

        [OneTimeSetUp]
        public void BeforeAll()
        {
            var capabilities = Caps.GetAndroidUIAutomatorCaps(Apps.Get("androidApiDemos"));
            capabilities.AddAdditionalAppiumOption("newCommandTimeout", 150);

            serverUri = new Uri("http://localhost:4723");
            _driver = new AndroidDriver(serverUri, capabilities, Env.InitTimeoutSec);
            _driver.Manage().Timeouts().ImplicitWait = Env.ImplicitTimeoutSec;
        }

        [OneTimeTearDown]
        public void AfterAll()
        {
            _driver?.Quit();
            if (!Env.ServerIsRemote())
            {
                AppiumServers.StopLocalService();
            }
        }

        [Test]
        public async Task VerifyLogcatListenerCanBeAssigned()
        {
            _driver.AddLogcatMessagesListener(msg => messageSemaphore.Release());
            _driver.AddLogcatConnectionListener(() => Console.WriteLine("Connected to the web socket"));
            _driver.AddLogcatDisconnectionListener(() => Console.WriteLine("Disconnected from the web socket"));
            _driver.AddLogcatErrorsListener(Console.Error.WriteLine);
            try
            {
                await _driver.StartLogcatBroadcast(serverUri.Host, serverUri.Port);
                messageSemaphore.Wait();
                _driver.BackgroundApp(TimeSpan.FromSeconds(1));
                _driver.LaunchApp();
                // Assert.True(_timeout.TotalMilliseconds, messageSemaphore.WaitOne(_timeout.Milliseconds));
                // Assert.That(_timeout.TotalMilliseconds, messageSemaphore.WaitOne(_timeout.Milliseconds));
                Assert.IsTrue(messageSemaphore.Wait(_timeout),$"Didn't receive any log message after {_timeout:h\\:mm\\:ss} timeout");
                //Assert.IsTrue(_timeout.TotalMilliseconds.Equals(messageSemaphore.Wait(_timeout.Milliseconds)),string.Format("Didn't recive any log message after %s timeout"));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Unexpected exception occurred", e);
            }
            finally
            {
                messageSemaphore.Release();
                _driver.StopLogcatBroadcast();
            }

        }

    }
}