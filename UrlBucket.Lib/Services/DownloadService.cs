using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MimeDetective;
using UrlBucket.Lib.Helper;
using UrlBucket.Lib.Models;

namespace UrlBucket.Lib.Services {
    public class DownloadService {

        public DownloadService() {
            if(!long.TryParse(Environment.GetEnvironmentVariable("SIZE_LIMIT"), out _sizeLimit)) {
                _sizeLimit = 3_145_728L;
            }
        }

        public async Task<FileModel> DownloadUrlAsync(Uri url, string userAgent = null) {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrEmpty(userAgent)) userAgent = null;
            var fm = new FileModel {
                ObjectName = url.ToObjectName(),
            };

            using (var wc = new WebClient()) {
                wc.DownloadProgressChanged += OnDownloadProgressChanged;
                var ua = userAgent ?? Environment.GetEnvironmentVariable("DOWNLOAD_USER_AGENT");
                if (!string.IsNullOrWhiteSpace(ua)) {
                    wc.Headers.Add("user-agent", ua);
                }

                fm.Content = await wc.DownloadDataTaskAsync(url);
                fm.ContentType = wc.ResponseHeaders["content-type"];
                if (string.IsNullOrEmpty(fm.ContentType) && fm.Content != null && fm.Content.Length > 1) {
                    var contentType = MimeTypes.GetFileType(() => fm.Content, null, fm.Content);
                    if (contentType != null && !string.IsNullOrWhiteSpace(contentType.Mime)) {
                        fm.ContentType = contentType.Mime;
                    }
                }
            }

            return fm;
        }

        private readonly long _sizeLimit;

        private void OnDownloadProgressChanged(object o, DownloadProgressChangedEventArgs ev) {
            var wc = (WebClient)o;
            if (ev.TotalBytesToReceive >= _sizeLimit) {
                Console.WriteLine($"totalSize >= sizeLimit ({ev.TotalBytesToReceive} >= {_sizeLimit})");
                wc.CancelAsync();
            }

            if (ev.BytesReceived >= _sizeLimit) {
                Console.WriteLine($"bytesReceived >= sizeLimit ({ev.BytesReceived} >= {_sizeLimit})");
                wc.CancelAsync();
            }
        }
    }
}
