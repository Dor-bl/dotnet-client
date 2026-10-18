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

using OpenQA.Selenium.Remote;
using System;
using System.Diagnostics;
using System.Net.Http;

namespace OpenQA.Selenium.Appium.Service
{
    public class AppiumHttpCommandExecutor(Uri addressOfRemoteServer, TimeSpan timeout, AppiumClientConfig clientConfig) : HttpCommandExecutor(addressOfRemoteServer, timeout, enableKeepAlive: true)
    {
        private readonly AppiumClientConfig _clientConfig = clientConfig;

        protected override HttpClientHandler CreateHttpClientHandler()
        {
            var handler = base.CreateHttpClientHandler();

            if (_clientConfig != null && _clientConfig.RelaxSslValidation)
            {
                Trace.TraceWarning("SSL validation is relaxed. This introduces security risks such as susceptibility to Man-In-The-Middle (MITM) attacks and should only be used in non-production environments.");
                handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
            }
            return handler;
        }

    }
}
