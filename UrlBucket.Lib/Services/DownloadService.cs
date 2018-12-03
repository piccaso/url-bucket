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
        public async Task<FileModel> DownloadUrlAsync(Uri url, string userAgent = null) {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrEmpty(userAgent)) userAgent = null;
            var fm = new FileModel {
                ObjectName = url.ToObjectName(),
            };

            using (var wc = new WebClient()) {
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
    }
}
