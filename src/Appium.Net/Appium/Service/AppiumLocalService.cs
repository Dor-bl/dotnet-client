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

using OpenQA.Selenium.Appium.Service.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace OpenQA.Selenium.Appium.Service
{
    /// <summary>
    /// Represents a local Appium server service that can be started and stopped programmatically.
    /// </summary>
    public class AppiumLocalService : IDisposable
    {
        private readonly FileInfo NodeJS;
        private readonly IReadOnlyList<string> ArgsList;
        private readonly IPAddress IP;
        private readonly int Port;
        private readonly TimeSpan InitializationTimeout;
        private readonly IDictionary<string, string> EnvironmentForProcess;
        private readonly HttpClient SharedHttpClient;
        private Process Service;

        private string Arguments => string.Join(" ", ArgsList ?? Array.Empty<string>());

        /// <summary>
        /// Creates an instance of AppiumLocalService without special settings
        /// </summary>
        /// <returns>An instance of AppiumLocalService without special settings</returns>
        public static AppiumLocalService BuildDefaultService() => new AppiumServiceBuilder().Build();

        internal AppiumLocalService(
            FileInfo nodeJS,
            IReadOnlyList<string> argsList,
            IPAddress ip,
            int port,
            TimeSpan initializationTimeout,
            IDictionary<string, string> environmentForProcess)
        {
            NodeJS = nodeJS;
            IP = ip;
            ArgsList = argsList ?? Array.Empty<string>();
            Port = port;
            InitializationTimeout = initializationTimeout;
            EnvironmentForProcess = environmentForProcess;
            SharedHttpClient = CreateHttpClientInstance();
        }

        internal AppiumLocalService(
            FileInfo nodeJS,
            string arguments,
            IPAddress ip,
            int port,
            TimeSpan initializationTimeout,
            IDictionary<string, string> environmentForProcess)
            : this(nodeJS, string.IsNullOrWhiteSpace(arguments) ? Array.Empty<string>() : (IReadOnlyList<string>)arguments.Split(' ').ToList().AsReadOnly(), ip, port, initializationTimeout, environmentForProcess)
        {
        }

        private static readonly System.Reflection.PropertyInfo ArgumentListProperty = typeof(ProcessStartInfo).GetProperty("ArgumentList");

        private static void PopulateArgumentList(ProcessStartInfo startInfo, IEnumerable<string> arguments)
        {
            if (ArgumentListProperty != null)
            {
                var list = ArgumentListProperty.GetValue(startInfo) as System.Collections.IList;
                if (list != null)
                {
                    foreach (var arg in arguments)
                    {
                        list.Add(arg);
                    }
                    return;
                }
            }

            startInfo.Arguments = FormatArgumentsForCommandLine(arguments);
        }

        private static string FormatArgumentsForCommandLine(IEnumerable<string> arguments)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var arg in arguments)
            {
                if (sb.Length > 0)
                {
                    sb.Append(' ');
                }
                sb.Append(EscapeArgument(arg));
            }
            return sb.ToString();
        }

        private static string EscapeArgument(string arg)
        {
            if (string.IsNullOrEmpty(arg))
            {
                return "\"\"";
            }

            if (arg.IndexOfAny(new[] { ' ', '\t', '"' }) < 0)
            {
                return arg;
            }

            var sb = new System.Text.StringBuilder();
            sb.Append('"');
            for (int i = 0; i < arg.Length; i++)
            {
                int backslashCount = 0;
                while (i < arg.Length && arg[i] == '\\')
                {
                    backslashCount++;
                    i++;
                }

                if (i == arg.Length)
                {
                    sb.Append('\\', backslashCount * 2);
                    break;
                }

                if (arg[i] == '"')
                {
                    sb.Append('\\', backslashCount * 2 + 1);
                    sb.Append('"');
                }
                else
                {
                    sb.Append('\\', backslashCount);
                    sb.Append(arg[i]);
                }
            }
            sb.Append('"');
            return sb.ToString();
        }

        private static HttpClient CreateHttpClientInstance()
        {
            return new HttpClient() { Timeout = TimeSpan.FromMinutes(2) };
        }

        /// <summary>
        /// The base URL for the managed appium server.
        /// </summary>
        public Uri ServiceUrl => new($"http://{IP}:{Convert.ToString(Port)}");

        /// <summary>
        /// Event that can be used to capture the output of the service
        /// </summary>
        public event DataReceivedEventHandler OutputDataReceived;

        /// <summary>
        /// Event that can be used to capture the error output of the service
        /// </summary>
        public event DataReceivedEventHandler ErrorDataReceived;

        /// <summary>
        /// Starts the defined Appium server.
        /// <remarks>
        /// <para>
        /// This method executes the synchronous version of starting the Appium server.
        /// </para>
        /// </remarks>
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Start()
        {
            StartAsync().GetAwaiter().GetResult();
        }

        private async Task StartAsync()
        {
            if (IsRunning)
            {
                return;
            }

            Service = new Process();
            Service.StartInfo.FileName = NodeJS.FullName;
            Service.StartInfo.UseShellExecute = false;
            Service.StartInfo.CreateNoWindow = true;

            if (ArgsList != null)
            {
                PopulateArgumentList(Service.StartInfo, ArgsList);
            }

            if (EnvironmentForProcess != null)
            {
                foreach (var entry in EnvironmentForProcess)
                {
                    Service.StartInfo.EnvironmentVariables[entry.Key] = entry.Value ?? string.Empty;
                }
            }

            Service.StartInfo.RedirectStandardOutput = true;
            Service.StartInfo.RedirectStandardError = true;
            Service.OutputDataReceived += (sender, e) => OutputDataReceived?.Invoke(this, e);
            Service.ErrorDataReceived += (sender, e) => ErrorDataReceived?.Invoke(this, e);

            bool isLaunched = false;
            string msgTxt =
                $"The local appium server has not been started. The given Node.js executable: {NodeJS.FullName} Arguments: {Arguments}. " +
                "\n";

            try
            {
                Service.Start();

                Service.BeginOutputReadLine();
                Service.BeginErrorReadLine();
            }
            catch (Exception e)
            {
                DestroyProcess();
                throw new AppiumServerHasNotBeenStartedLocallyException(msgTxt, e);
            }

            isLaunched = await PingAsync(InitializationTimeout).ConfigureAwait(false);
            if (!isLaunched)
            {
                DestroyProcess();
                throw new AppiumServerHasNotBeenStartedLocallyException(
                    msgTxt +
                    $"Time {InitializationTimeout.TotalMilliseconds} ms for the service starting has been expired!");
            }
        }

        private bool TryGracefulShutdownOnWindows(Process process, int timeoutMs = 5000)
        {
            if (process == null)
                return true;

            // Safely check HasExited, handling disposed process
            bool hasExited;
            try
            {
                hasExited = process.HasExited;
            }
            catch (InvalidOperationException)
            {
                // Process is disposed, treat as exited
                return true;
            }

            if (hasExited)
                return true;

            // Attempt graceful shutdown using managed code only
            try
            {
                process.CloseMainWindow();
                if (process.WaitForExit(timeoutMs))
                    return true;
            }
            catch
            {
                // Ignore exceptions, fallback to Kill
            }

            return false;
        }

        private int GetShutdownTimeoutWithBuffer()
        {
            // Default Appium shutdown timeout in ms
            int shutdownTimeout = 5000;
            const int bufferMs = 1000;

            if (ArgsList != null)
            {
                int idx = -1;
                for (int i = 0; i < ArgsList.Count; i++)
                {
                    if (ArgsList[i] == "--shutdown-timeout")
                    {
                        idx = i;
                        break;
                    }
                }
                if (idx >= 0 && idx + 1 < ArgsList.Count)
                {
                    if (int.TryParse(ArgsList[idx + 1], out int parsed))
                        shutdownTimeout = parsed;
                }
            }

            return shutdownTimeout + bufferMs;
        }

        private void DestroyProcess()
        {
            if (Service == null)
                return;

            try
            {
                int shutdownTimeout = GetShutdownTimeoutWithBuffer();

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // Attempt graceful shutdown on Windows
                    if (!TryGracefulShutdownOnWindows(Service, shutdownTimeout))
                    {
                        Service.Kill();
                    }
                }
                else
                {
                    // On non-Windows, just kill the process
                    Service.Kill();
                }
            }
            catch
            {
                // Optionally log or handle exceptions
            }
            finally
            {
                Service?.Close();
                SharedHttpClient.Dispose();
            }
        }

        /// <summary>
        /// Stops this service if it is currently running.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Dispose()
        {
            DestroyProcess();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Is the defined appium server being run or not
        /// </summary>
        public bool IsRunning
        {
            get
            {
                if (Service == null)
                {
                    return false;
                }

                try
                {
                    var pid = Service.Id;
                }
                catch (Exception)
                {
                    return false;
                }

                return IsRunningAsync(TimeSpan.FromMilliseconds(500)).GetAwaiter().GetResult();
            }
        }

        private async Task<bool> IsRunningAsync(TimeSpan timeout)
        {
            return await PingAsync(timeout).ConfigureAwait(false);
        }

        private string GetArgsValue(string argStr)
        {
            if (ArgsList != null)
            {
                for (int i = 0; i < ArgsList.Count; i++)
                {
                    if (ArgsList[i] == argStr && i + 1 < ArgsList.Count)
                    {
                        return ArgsList[i + 1];
                    }
                }
            }
            return null;
        }

        private string ParseBasePath()
        {
            if (ArgsList != null)
            {
                for (int i = 0; i < ArgsList.Count; i++)
                {
                    if (ArgsList[i] == "--base-path" || ArgsList[i] == "-pa")
                    {
                        if (i + 1 < ArgsList.Count)
                        {
                            return ArgsList[i + 1];
                        }
                    }
                }
            }
            return AppiumServiceConstants.DefaultBasePath;
        }

        private Uri CreateStatusUrl()
        {
            Uri status;
            Uri service = ServiceUrl;

            string basePath = ParseBasePath();
            bool defBasePath = basePath.Equals(AppiumServiceConstants.DefaultBasePath);

            if (service.IsLoopback || IP.ToString().Equals(AppiumServiceConstants.DefaultLocalIPAddress))
            {
                string tmpStatus = "http://localhost:" + Convert.ToString(Port);
                if (defBasePath)
                {
                    status = new Uri(tmpStatus + AppiumServiceConstants.StatusUrl);
                }
                else
                {
                    status = new Uri(tmpStatus + basePath + AppiumServiceConstants.StatusUrl);
                }
            }
            else
            {
                if (defBasePath)
                {
                    status = new Uri(service, AppiumServiceConstants.StatusUrl);
                }
                else
                {
                    status = new Uri(service, basePath + AppiumServiceConstants.StatusUrl);
                }
            }
            return status;
        }

        private async Task<bool> PingAsync(TimeSpan span)
        {
            bool pinged = false;

            Uri status;

            status = CreateStatusUrl();

            DateTime endTime = DateTime.Now.Add(span);
            while (!pinged && DateTime.Now < endTime)
            {
                try
                {
                    using HttpResponseMessage response = await SharedHttpClient.GetAsync(status).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                }
                catch
                {
                    pinged = false;
                }
                await Task.Delay(250).ConfigureAwait(false);
            }
            return pinged;
        }
    }
}