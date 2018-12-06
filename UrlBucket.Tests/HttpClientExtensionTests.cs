using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using UrlBucket.Lib.Helper;

namespace UrlBucket.Tests {
    public class HttpClientExtensionTests {

        private const long DownloadSize1Mb = 1048576;

        [Test]
        public async Task UserAgent() {
            var hc = new HttpClient();
            var r = await hc.SendAsync(HttpMethod.Get, new Uri("http://httpbin.org/get"), "lala/1");
            var responseText = Encoding.UTF8.GetString(r.Content);
            TestContext.WriteLine(responseText);
            Assert.IsTrue(responseText.Contains("\"User-Agent\": \"lala/1\""));
        }

        [Test]
        public void ContentLengthSizeLimit() {
            var hc = new HttpClient();
            Assert.Throws<InvalidOperationException>(() => {
                hc.SendAsync(HttpMethod.Get, new Uri("http://speed.hetzner.de/10GB.bin"), null, DownloadSize1Mb, true).GetAwaiter().GetResult();
            });
        }

        [Test]
        public void DownloadSizeLimit() {
            var hc = new HttpClient();
            Assert.Throws<InvalidOperationException>(() => {
                hc.SendAsync(HttpMethod.Get, new Uri("http://speed.hetzner.de/10GB.bin"), null, DownloadSize1Mb, false).GetAwaiter().GetResult();
            });
        }

        [Test]
        public async Task Timeout() {
            var hc = new HttpClient();
            var cts = new CancellationTokenSource(1);
            var cnt = 0;
            try {
                await hc.SendAsync(HttpMethod.Get, new Uri("http://speed.hetzner.de/10GB.bin"), null, long.MaxValue, ct: cts.Token);
            } catch (TaskCanceledException) {
                cnt++;
            } catch (OperationCanceledException) {
                cnt++;
            }

            Assert.AreEqual(1,cnt);
        }
    }
}
