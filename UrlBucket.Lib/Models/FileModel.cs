using System;
using Microsoft.Build.Framework;

namespace UrlBucket.Lib.Models {

    public class BaseFileModel {
        [Required]
        public byte[] Content { get; set; }
        public string ContentType { get; set; }
    }

    public class FileModel : BaseFileModel {
        public string ObjectName;
    }

    public class DownloadFileModel : FileModel {
        public string Etag { get; set; }
    }

    public class UploadFileModel : BaseFileModel {
        [Required]
        public Uri Url { get; set; }
    }
}
