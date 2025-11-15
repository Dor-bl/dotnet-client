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

using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.Interfaces;
using OpenQA.Selenium.Appium.Service;
using OpenQA.Selenium.Appium.WebSocket;
using System;
using System.Drawing;
using OpenQA.Selenium.Appium.iOS.Interfaces;
using System.Collections.Generic;

namespace OpenQA.Selenium.Appium.iOS
{
    public class IOSDriver : AppiumDriver, IHidesKeyboardWithKeyName, IHasClipboard,
        IShakesDevice, IPerformsTouchID, IHasSettings, IListensToSyslogMessages
    {
        private static readonly string Platform = MobilePlatform.IOS;
        private static readonly StringWebSocketClient SyslogClient = new StringWebSocketClient();
        private const int DefaultAppiumPort = 4723;

        /// <summary>
        /// Initializes a new instance of the IOSDriver class
        /// </summary>
        /// <param name="commandExecutor">An <see cref="ICommandExecutor"/> object which executes commands for the driver.</param>
        /// <param name="driverOptions">An <see cref="DriverOptions"/> object containing the Appium options.</param>
        public IOSDriver(ICommandExecutor commandExecutor, DriverOptions driverOptions)
            : base(commandExecutor, SetPlatformToCapabilities(driverOptions, Platform))
        {
        }

        /// <summary>
        /// Initializes a new instance of the IOSDriver class using Appium options
        /// </summary>
        /// <param name="driverOptions">An <see cref="DriverOptions"/> object containing the Appium options of the browser.</param>
        public IOSDriver(DriverOptions driverOptions)
            : base(SetPlatformToCapabilities(driverOptions, Platform))
        {
        }

        /// <summary>
        /// Initializes a new instance of the IOSDriver class using Appium options and command timeout
        /// </summary>
        /// <param name="driverOptions">An <see cref="DriverOptions"/> object containing the Appium options.</param>
        /// <param name="commandTimeout">The maximum amount of time to wait for each command.</param>
        public IOSDriver(DriverOptions driverOptions, TimeSpan commandTimeout)
            : base(SetPlatformToCapabilities(driverOptions, Platform), commandTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the IOSDriver class using the AppiumServiceBuilder instance and Appium options
        /// </summary>
        /// <param name="builder">The object containing settings of the Appium local service which is going to be started.</param>
        /// <param name="driverOptions">An <see cref="DriverOptions"/> object containing the Appium options.</param>
        public IOSDriver(AppiumServiceBuilder builder, DriverOptions driverOptions)
            : base(builder, SetPlatformToCapabilities(driverOptions, Platform))
        {
        }

        /// <summary>
        /// Initializes a new instance of the IOSDriver class using the AppiumServiceBuilder instance, Appium options and command timeout
        /// </summary>
        /// <param name="builder">The object containing settings of the Appium local service which is going to be started.</param>
        /// <param name="driverOptions">An <see cref="DriverOptions"/> object containing the Appium options.</param>
        /// <param name="commandTimeout">The maximum amount of time to wait for each command.</param>
        public IOSDriver(AppiumServiceBuilder builder, DriverOptions driverOptions, TimeSpan commandTimeout)
            : base(builder, SetPlatformToCapabilities(driverOptions, Platform), commandTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the IOSDriver class using the specified remote address and Appium options
        /// </summary>
        /// <param name="remoteAddress">URI containing the address of the WebDriver remote server (e.g. http://127.0.0.1:4723/).</param>
        /// <param name="driverOptions">An <see cref="DriverOptions"/> object containing the Appium options.</param>
        public IOSDriver(Uri remoteAddress, DriverOptions driverOptions)
            : base(remoteAddress, SetPlatformToCapabilities(driverOptions, Platform))
        {
        }

        /// <summary>
        /// Initializes a new instance of the IOSDriver class using the specified Appium local service and Appium options
        /// </summary>
        /// <param name="service">The specified Appium local service.</param>
        /// <param name="driverOptions">An <see cref="DriverOptions"/> object containing the Appium options of the browser.</param>
        public IOSDriver(AppiumLocalService service, DriverOptions driverOptions)
            : base(service, SetPlatformToCapabilities(driverOptions, Platform))
        {
        }

        /// <summary>
        /// Initializes a new instance of the IOSDriver class using the specified remote address, Appium options, and command timeout.
        /// </summary>
        /// <param name="remoteAddress">URI containing the address of the WebDriver remote server (e.g. http://127.0.0.1:4723/).</param>
        /// <param name="driverOptions">An <see cref="DriverOptions"/> object containing the Appium options.</param>
        /// <param name="commandTimeout">The maximum amount of time to wait for each command.</param>
        public IOSDriver(Uri remoteAddress, DriverOptions driverOptions, TimeSpan commandTimeout)
            : base(remoteAddress, SetPlatformToCapabilities(driverOptions, Platform), commandTimeout)
        {
        }

        /// <summary>
        /// Initializes a new instance of the IOSDriver class using the specified Appium local service, Appium options, and command timeout.
        /// </summary>
        /// <param name="service">The specified Appium local service.</param>
        /// <param name="driverOptions">An <see cref="DriverOptions"/> object containing the Appium options.</param>
        /// <param name="commandTimeout">The maximum amount of time to wait for each command.</param>
        public IOSDriver(AppiumLocalService service, DriverOptions driverOptions, TimeSpan commandTimeout)
            : base(service, SetPlatformToCapabilities(driverOptions, Platform), commandTimeout)
        {
        }


        /// <summary>
        /// Initializes a new instance of the IOSDriver class using the specified remote address, Appium options and AppiumClientConfig.
        /// </summary>
        /// <param name="remoteAddress">URI containing the address of the WebDriver remote server (e.g. http://127.0.0.1:4723/).</param>
        /// <param name="driverOptions">An <see cref="DriverOptions"/> object containing the Appium options.</param>
        /// <param name="clientConfig">An instance of <see cref="AppiumClientConfig"/></param>
        public IOSDriver(Uri remoteAddress, DriverOptions driverOptions, AppiumClientConfig clientConfig)
            : base(remoteAddress, SetPlatformToCapabilities(driverOptions, Platform), clientConfig)
        {
        }

        /// <summary>
        /// Initializes a new instance of the IOSDriver class using the specified Appium local service, Appium options and AppiumClientConfig,
        /// </summary>
        /// <param name="service">The specified Appium local service.</param>
        /// <param name="driverOptions">An <see cref="ICapabilities"/> object containing the Appium options.</param>
        /// <param name="clientConfig">An instance of <see cref="AppiumClientConfig"/></param>
        public IOSDriver(AppiumLocalService service, DriverOptions driverOptions, AppiumClientConfig clientConfig)
            : base(service, SetPlatformToCapabilities(driverOptions, Platform), clientConfig)
        {
        }

        /// <summary>
        /// Initializes a new instance of the IOSDriver class using the specified remote address, Appium options, command timeout and AppiumClientConfig.
        /// </summary>
        /// <param name="remoteAddress">URI containing the address of the WebDriver remote server (e.g. http://127.0.0.1:4723/).</param>
        /// <param name="driverOptions">An <see cref="DriverOptions"/> object containing the Appium options.</param>
        /// <param name="commandTimeout">The maximum amount of time to wait for each command.</param>
        /// <param name="clientConfig">An instance of <see cref="AppiumClientConfig"/></param>
        public IOSDriver(Uri remoteAddress, DriverOptions driverOptions, TimeSpan commandTimeout, AppiumClientConfig clientConfig)
            : base(remoteAddress, SetPlatformToCapabilities(driverOptions, Platform), commandTimeout, clientConfig)
        {
        }

        /// <summary>
        /// Initializes a new instance of the IOSDriver class using the specified Appium local service, Appium options, command timeout and AppiumClientConfig,
        /// </summary>
        /// <param name="service">The specified Appium local service.</param>
        /// <param name="driverOptions">An <see cref="ICapabilities"/> object containing the Appium options.</param>
        /// <param name="commandTimeout">The maximum amount of time to wait for each command.</param>
        /// <param name="clientConfig">An instance of <see cref="AppiumClientConfig"/></param>
        public IOSDriver(AppiumLocalService service, DriverOptions driverOptions, TimeSpan commandTimeout, AppiumClientConfig clientConfig)
            : base(service, SetPlatformToCapabilities(driverOptions, Platform), commandTimeout, clientConfig)
        {
        }

        public void SetSetting(string setting, object value) =>
            IOSCommandExecutionHelper.SetSetting(this, setting, value);

        public Dictionary<string, object> Settings
        {
            get => IOSCommandExecutionHelper.GetSettings(this);

            set
            {
                foreach (var entry in value)
                {
                    SetSetting(entry.Key, entry.Value);
                }
            }
        }

        /// <summary>
        /// Shakes the device. This works only for Simulator.
        /// </summary>
        public void ShakeDevice() => IOSCommandExecutionHelper.ShakeDevice(this);

        /// <summary>
        /// Hides the keyboard with the given key.
        /// </summary>
        /// <param name="key">The key to hide the keyboard with.</param>
        public new void HideKeyboard(string key) =>
            AppiumCommandExecutionHelper.HideKeyboard(this, key);

        /// <summary>
        /// Performs Touch ID authentication.
        /// </summary>
        /// <param name="match">Whether to simulate biometric match.</param>
        public void PerformTouchID(bool match) => IOSCommandExecutionHelper.PerformTouchID(this, match);

        /// <summary>
        /// Check if the device is locked
        /// </summary>
        /// <returns>true if device is locked, false otherwise</returns>
        public bool IsLocked() => (bool)ExecuteScript("mobile: isLocked");

        /// <summary>
        /// Locks the device. Optionally, unlocks it after a specified number of seconds.
        /// </summary>
        /// <param name="seconds">
        /// The number of seconds after which the device will be automatically unlocked.
        /// Set to 0 or leave it empty to require manual unlock.
        /// </param>
        /// <exception cref="WebDriverException">Thrown if the command execution fails.</exception>
        public void Lock(int? seconds = null)
        {
            var parameters = new Dictionary<string, object>();

            if (seconds.HasValue && seconds.Value > 0)
            {
                parameters["seconds"] = seconds.Value;
            }

            ExecuteScript("mobile: lock", parameters);
        }

        public void Unlock() => ExecuteScript("mobile: unlock");

        /// <summary>
        /// Sets the content to the clipboard
        /// </summary>
        /// <param name="contentType">An <see cref="ClipboardContentType"/> object to set (PlainText, Image or Url).</param>
        /// <param name="base64Content">The base64-encoded content to set.</param>
        public void SetClipboard(ClipboardContentType contentType, string base64Content) =>
            AppiumCommandExecutionHelper.MobileSetClipboard(this, contentType, base64Content);

        /// <summary>
        /// Get the content of the clipboard.
        /// </summary>
        /// <param name="contentType">An <see cref="ClipboardContentType"/> object (PlainText, Image or Url).</param>
        /// <remarks>Android supports plaintext only</remarks>
        /// <returns>The content of the clipboard as base64-encoded string or an empty string if the clipboard is empty</returns>
        public string GetClipboard(ClipboardContentType contentType) =>
            AppiumCommandExecutionHelper.MobileGetClipboard(this, contentType);

        /// <summary>
        /// Sets text to the clipboard
        /// </summary>
        /// <param name="textContent">The text content.</param>
        /// <param name="label">For Android only - A user visible label for the clipboard content.</param>
        public void SetClipboardText(string textContent, string label = null) =>
            AppiumCommandExecutionHelper.SetClipboardText(this, textContent, null);

        /// <summary>
        /// Get the plaintext content of the clipboard.
        /// </summary>
        /// <remarks>Android supports plaintext only</remarks>
        /// <returns>The string content of the clipboard or an empty string if the clipboard is empty</returns>
        public string GetClipboardText() =>
            AppiumCommandExecutionHelper.GetClipboardText(this);

        /// <summary>
        /// Sets the url string to the clipboard
        /// </summary>
        /// <param name="url">The URL string.</param>
        public void SetClipboardUrl(string url) => IOSCommandExecutionHelper.SetClipboardUrl(this, url);

        /// <summary>
        /// Gets the url string from the clipboard
        /// </summary>
        /// <returns>The url string content of the clipboard or an empty string if the clipboard is empty</returns>
        public string GetClipboardUrl() => IOSCommandExecutionHelper.GetClipboardUrl(this);

        /// <summary>
        /// Sets the image to the clipboard
        /// </summary>
        /// <param name="image">The image object.</param>
        public void SetClipboardImage(Image image) => IOSCommandExecutionHelper.SetClipboardImage(this, image);

        /// <summary>
        /// Gets the image from the clipboard
        /// </summary>
        /// <returns>The image content of the clipboard as an Image object or null if there is no image on the clipboard</returns>
        public Image GetClipboardImage() => IOSCommandExecutionHelper.GetClipboardImage(this);


        /// <summary>
        /// Gets the State of the app.
        /// </summary>
        /// <param name="bundleId">A string containing the id of the app.</param>
        /// <returns>an enumeration of the app state.</returns>
         public AppState GetAppState(string bundleId) =>
            (AppState)Convert.ToInt32(
                ExecuteScript(
                    "mobile:queryAppState",
                    new object[] {
                        new Dictionary<string, object>{
                            ["bundleId"] = bundleId
                        }
                    }
                ).ToString()
            );

        #region Syslog Broadcast

        /// <summary>
        /// Start syslog messages broadcast via web socket.
        /// This method assumes that Appium server is running on localhost and
        /// is assigned to the default port (4723).
        /// </summary>
        public void StartSyslogBroadcast() => StartSyslogBroadcast("localhost", DefaultAppiumPort);

        /// <summary>
        /// Start syslog messages broadcast via web socket.
        /// This method assumes that Appium server is assigned to the default port (4723).
        /// </summary>
        /// <param name="host">The name of the host where Appium server is running.</param>
        public void StartSyslogBroadcast(string host) => StartSyslogBroadcast(host, DefaultAppiumPort);

        /// <summary>
        /// Start syslog messages broadcast via web socket.
        /// </summary>
        /// <param name="host">The name of the host where Appium server is running.</param>
        /// <param name="port">The port of the host where Appium server is running.</param>
        public void StartSyslogBroadcast(string host, int port)
        {
            ExecuteScript("mobile: startLogsBroadcast", new Dictionary<string, object>());
            var endpointUri = new Uri($"ws://{host}:{port}/ws/session/{SessionId}/appium/device/syslog");
            SyslogClient.ConnectAsync(endpointUri).Wait();
        }

        /// <summary>
        /// Adds a new log messages broadcasting handler.
        /// Several handlers might be assigned to a single server.
        /// Multiple calls to this method will cause such handler
        /// to be called multiple times.
        /// </summary>
        /// <param name="handler">A function, which accepts a single argument, which is the actual log message.</param>
        public void AddSyslogMessagesListener(Action<string> handler) => 
            SyslogClient.MessageHandlers.Add(handler);

        /// <summary>
        /// Adds a new log broadcasting errors handler.
        /// Several handlers might be assigned to a single server.
        /// Multiple calls to this method will cause such handler
        /// to be called multiple times.
        /// </summary>
        /// <param name="handler">A function, which accepts a single argument, which is the actual exception instance.</param>
        public void AddSyslogErrorsListener(Action<Exception> handler) => 
            SyslogClient.ErrorHandlers.Add(handler);

        /// <summary>
        /// Adds a new log broadcasting connection handler.
        /// Several handlers might be assigned to a single server.
        /// Multiple calls to this method will cause such handler
        /// to be called multiple times.
        /// </summary>
        /// <param name="handler">A function, which is executed as soon as the client is successfully connected to the web socket.</param>
        public void AddSyslogConnectionListener(Action handler) => 
            SyslogClient.ConnectionHandlers.Add(handler);

        /// <summary>
        /// Adds a new log broadcasting disconnection handler.
        /// Several handlers might be assigned to a single server.
        /// Multiple calls to this method will cause such handler
        /// to be called multiple times.
        /// </summary>
        /// <param name="handler">A function, which is executed as soon as the client is successfully disconnected from the web socket.</param>
        public void AddSyslogDisconnectionListener(Action handler) => 
            SyslogClient.DisconnectionHandlers.Add(handler);

        /// <summary>
        /// Removes all existing syslog handlers.
        /// </summary>
        public void RemoveAllSyslogListeners() => SyslogClient.RemoveAllHandlers();

        /// <summary>
        /// Stops syslog messages broadcast via web socket.
        /// </summary>
        public void StopSyslogBroadcast()
        {
            ExecuteScript("mobile: stopLogsBroadcast", new Dictionary<string, object>());
            SyslogClient.DisconnectAsync().Wait();
        }

        #endregion
    }
}