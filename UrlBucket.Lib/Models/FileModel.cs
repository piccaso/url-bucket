using System;
using System.ComponentModel.DataAnnotations;

namespace UrlBucket.Lib.Models {

    public class BaseFileContentTypeModel {
        public string ContentType { get; set; }
    }

    public class BaseFileContentModel : BaseFileContentTypeModel {
        [Required]
        public byte[] Content { get; set; }
    }

    public class FileModel : BaseFileContentModel {
        public string ObjectName;
    }

    public class DownloadFileModel : FileModel {
        public string Etag { get; set; }
    }

    public class UploadFileModel : BaseFileContentModel {
        [Required]
        public Uri Url { get; set; }
    }
}
