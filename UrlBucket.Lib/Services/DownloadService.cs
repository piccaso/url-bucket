using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MimeDetective;
using UrlBucket.Lib.Helper;
using UrlBucket.Lib.Models;

namespace UrlBucket.Lib.Services {
    public class DownloadService {
        private readonly IConfig _config;
        public DownloadService(IConfig config) {
            _config = config;
            if(!long.TryParse(_config.GetValue("SIZE_LIMIT"), out _sizeLimit)) {
                _sizeLimit = 1_048_576; // 1MB
            }
        }

        public async Task<FileModel> DownloadUrlAsync(Uri url, string userAgent = null, CancellationToken ct = default(CancellationToken)) {
            if (url == null) throw new ArgumentNullException(nameof(url));
            if (string.IsNullOrEmpty(userAgent)) userAgent = null;
            var fm = new FileModel {
                ObjectName = url.ToObjectName(),
            };
            using (var httpClient = new HttpClient()) {
                var ua = userAgent ?? _config.GetValue("DOWNLOAD_USER_AGENT");
                var response = await httpClient.SendAsync(HttpMethod.Get, url, ua, _sizeLimit, ct: ct);
                fm.Content = response.Content;
                fm.ContentType = response.ContentType;
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
    }
}
