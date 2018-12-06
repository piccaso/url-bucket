using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UrlBucket.Lib.Models;

namespace UrlBucket.Lib.Helper {
    public static class HttpClientExtensions {
        public static async Task<BaseFileContentModel> SendAsync(this HttpClient httpClient, HttpMethod method, Uri url, string userAgent = null, long maxSize = 1048576, bool useContentLengthHeader = true, int buffSiz = 8192, CancellationToken ct = default(CancellationToken)) {

            if (!string.IsNullOrWhiteSpace(userAgent)) {
                httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
            }

            var content = new BaseFileContentModel();
            var buffer     = new byte[buffSiz];
            var totalBytes = 0L;

            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(ct)) {
                    using (var ms = new MemoryStream())
                    using (var request = new HttpRequestMessage(method, url))
                    using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cts.Token))
                    using (var responseStream = await response.Content.ReadAsStreamAsync())
                    {
                        if (useContentLengthHeader && response.Content.Headers.ContentLength.HasValue && response.Content.Headers.ContentLength.Value > maxSize) {
                            cts.Cancel();
                            throw new InvalidOperationException("content length exceeds max size");
                        }

                        while (!cts.Token.IsCancellationRequested) {
                            var bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length, cts.Token);
                            if(bytesRead == 0) break;
                            totalBytes += bytesRead;
                            if (totalBytes >= maxSize) {
                                cts.Cancel();
                                throw new InvalidOperationException("downloaded content exceeds max size");
                            }
                            await ms.WriteAsync(buffer, 0, bytesRead, cts.Token);
                        }

                        content.ContentType = response?.Content?.Headers?.ContentType?.MediaType;
                        content.Content = ms.ToArray();
                    }
            }

            return content;
        }
    }
}
