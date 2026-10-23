using System;
using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium.Appium.WebSocket;

namespace Appium.Net.Integration.Tests.WebSocket
{
    [TestFixture]
    public class StringWebSocketClientTests
    {
        private HttpListener _httpListener;
        private string _serverUri;
        private CancellationTokenSource _listenerCts;

        [OneTimeSetUp]
        public void StartMockServer()
        {
            int port = 8089;
            _serverUri = $"ws://127.0.0.1:{port}/ws/";
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add($"http://127.0.0.1:{port}/ws/");
            _httpListener.Start();
            _listenerCts = new CancellationTokenSource();

            Task.Run(async () =>
            {
                while (!_listenerCts.Token.IsCancellationRequested)
                {
                    try
                    {
                        var context = await _httpListener.GetContextAsync();
                        if (context.Request.IsWebSocketRequest)
                        {
                            var wsContext = await context.AcceptWebSocketAsync(null);
                            var buffer = new byte[1024];
                            while (wsContext.WebSocket.State == WebSocketState.Open && !_listenerCts.Token.IsCancellationRequested)
                            {
                                try
                                {
                                    var result = await wsContext.WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _listenerCts.Token);
                                    if (result.MessageType == WebSocketMessageType.Close)
                                    {
                                        await wsContext.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                                        break;
                                    }
                                }
                                catch
                                {
                                    break;
                                }
                            }
                        }
                    }
                    catch
                    {
                        break;
                    }
                }
            });
        }

        [OneTimeTearDown]
        public void StopMockServer()
        {
            _listenerCts?.Cancel();
            _listenerCts?.Dispose();
            _httpListener?.Stop();
            _httpListener?.Close();
        }

        [Test]
        public async Task Dispose_ConnectedClient_BenchmarkDisposalTime()
        {
            const int iterations = 50;
            var sw = Stopwatch.StartNew();

            for (int i = 0; i < iterations; i++)
            {
                var client = new StringWebSocketClient();
                await client.ConnectAsync(new Uri(_serverUri));
                client.Dispose();
            }

            sw.Stop();
            double avgMs = sw.Elapsed.TotalMilliseconds / iterations;
            Console.WriteLine($"[BENCHMARK] Total time for {iterations} connect & dispose cycles: {sw.ElapsedMilliseconds} ms (Avg per dispose: {avgMs:F2} ms)");
        }

        [Test]
        public void Dispose_UnconnectedClient_CompletesFastWithoutDeadlock()
        {
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000; i++)
            {
                var client = new StringWebSocketClient();
                client.Dispose();
            }
            sw.Stop();
            Console.WriteLine($"[BENCHMARK] Disposed 1000 unconnected clients in {sw.ElapsedMilliseconds} ms");
            Assert.That(sw.ElapsedMilliseconds, Is.LessThan(5000));
        }

        [Test]
        public void Dispose_CanBeCalledMultipleTimesSafely()
        {
            var client = new StringWebSocketClient();
            Assert.DoesNotThrow(() =>
            {
                client.Dispose();
                client.Dispose();
                client.Dispose();
            });
        }

        [Test]
        public void Dispose_OnSingleThreadedSynchronizationContext_DoesNotDeadlock()
        {
            var originalContext = SynchronizationContext.Current;
            try
            {
                var customContext = new TestSingleThreadedSynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(customContext);

                var task = Task.Run(() =>
                {
                    var client = new StringWebSocketClient();
                    client.Dispose();
                });

                bool completed = task.Wait(TimeSpan.FromSeconds(3));
                Assert.That(completed, Is.True, "Dispose deadlocked on SynchronizationContext!");
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(originalContext);
            }
        }

        private class TestSingleThreadedSynchronizationContext : SynchronizationContext
        {
            public override void Post(SendOrPostCallback d, object state)
            {
                d(state);
            }
        }
    }
}
