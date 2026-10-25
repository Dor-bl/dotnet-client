//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//See the NOTICE file distributed with this work for additional
//information regarding copyright ownership.
//You may obtain a copy of the License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

using System;
using System.Collections.Generic;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Windows;

namespace Appium.Net.Integration.Tests.Windows
{
    public class WindowsDriverTests
    {
        private class DummyCommandExecutor : ICommandExecutor
        {
            public Response Execute(Command command)
            {
                if (command.Name == DriverCommand.NewSession)
                {
                    return new Response("dummy-session-id", new Dictionary<string, object>(), WebDriverResult.Success);
                }
                return new Response("dummy-session-id", null, WebDriverResult.Success);
            }

            public System.Threading.Tasks.Task<Response> ExecuteAsync(Command command) =>
                System.Threading.Tasks.Task.FromResult(Execute(command));

            public bool TryAddCommand(string commandName, CommandInfo commandInfo) => true;

            public void Dispose() { }
        }

        private WindowsDriver _driver;

        [SetUp]
        public void SetUp()
        {
            var options = new AppiumOptions();
            _driver = new WindowsDriver(new DummyCommandExecutor(), options);
        }

        [TearDown]
        public void TearDown()
        {
            _driver?.Dispose();
        }

        [Test]
        public void PressKeyCodeWithKeyEventThrowsNotSupportedException()
        {
            var keyEvent = new KeyEvent();
            Assert.Throws<NotSupportedException>(() => _driver.PressKeyCode(keyEvent));
        }

        [Test]
        public void LongPressKeyCodeWithKeyEventThrowsNotSupportedException()
        {
            var keyEvent = new KeyEvent();
            Assert.Throws<NotSupportedException>(() => _driver.LongPressKeyCode(keyEvent));
        }

        [Test]
        public void SetContextThrowsNotSupportedException()
        {
            Assert.Throws<NotSupportedException>(() => _driver.Context = "NATIVE_APP");
        }
    }
}
