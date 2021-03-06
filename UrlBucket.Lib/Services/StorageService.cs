﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Minio;
using Minio.Exceptions;
using UrlBucket.Lib.Helper;
using UrlBucket.Lib.Models;

namespace UrlBucket.Lib.Services {
    public class StorageService {
        private readonly IConfig _config;
        public StorageService(IConfig config) {
            _config = config;
        }

        private string _bucketName;
        private static bool? _bucketExists;
        private MinioClient CreateClient() {

            var endpoint = _config.GetValue("MINIO_ENDPOINT", "play.minio.io:9000");
            var accessKey = _config.GetValue("MINIO_ACCESS_KEY", "Q3AM3UQ867SPQQA43P2F");
            var secretKey = _config.GetValue("MINIO_SECRET_KEY", "zuf+tfteSlswRu7BJ86wekitnifILbZam1KYY3TG");
            var region = _config.GetValue("MINIO_REGION", "");
            _bucketName = _config.GetValue("MINIO_BUCKET_NAME", "combinary");
            var ssl = bool.Parse(_config.GetValue("MINIO_SSL", "True"));

            var minio = new MinioClient(endpoint, accessKey, secretKey, region);
            if (ssl) minio = minio.WithSSL();

            return minio;
        }

        public async Task UploadFileAsync(UploadFileModel file, CancellationToken ct = default(CancellationToken)) {
            await UploadFileAsync(file.Url.ToObjectName(), file.Content, file.ContentType, null, ct);
        }

        public async Task UploadFileAsync(FileModel file, CancellationToken ct = default(CancellationToken)) {
            await UploadFileAsync(file.ObjectName, file.Content, file.ContentType, null, ct);
        }

        public async Task UploadFileAsync(string objectName, byte[] content, string contentType = null, IDictionary<string,string> metaData = null, CancellationToken ct = default(CancellationToken)) {
            var client = CreateClient();

            var exists = _bucketExists ?? await client.BucketExistsAsync(_bucketName, ct);
            if (!exists) {
                await client.MakeBucketAsync(_bucketName, cancellationToken: ct);
                _bucketExists = true;
            }

            using (var ms = new MemoryStream(content)) {
                await client.PutObjectAsync(_bucketName, objectName, ms, ms.Length, contentType, metaData.ToDictionary(), cancellationToken: ct);
            }
        }

        public async Task<bool> FileExistsAsync(Uri url, CancellationToken ct = default(CancellationToken)) {
            var objectName = url.ToObjectName();
            return await FileExistsAsync(objectName, ct);
        }

        public async Task<bool> FileExistsAsync(string objectName, CancellationToken ct = default(CancellationToken)) {
            try {
                var client = CreateClient();
                var stat = await client.StatObjectAsync(_bucketName, objectName, cancellationToken: ct);
                return stat.Size > 0;
            }
            catch (BucketNotFoundException) {
                return false;
            }
            catch (ObjectNotFoundException) {
                return false;
            }
        }

        public async Task<DownloadFileModel> DownloadFileAsync(Uri url, CancellationToken ct = default(CancellationToken)) {
            var objectName = url.ToObjectName();
            return await DownloadFileAsync(objectName, ct);
        }

        public async Task<DownloadFileModel> DownloadFileAsync(string objectName, CancellationToken ct = default(CancellationToken)) {
            try {
                var client = CreateClient();
                var statTask = client.StatObjectAsync(_bucketName, objectName, cancellationToken: ct);
                var fm = new DownloadFileModel();
                using (var ms = new MemoryStream()) {
                    await client.GetObjectAsync(_bucketName, objectName, s => {
                        // ReSharper disable once AccessToDisposedClosure
                        s.CopyTo(ms);
                    }, cancellationToken: ct);
                    fm.Content = ms.ToArray();
                }

                var stat = await statTask;
                fm.ContentType = stat.ContentType;
                fm.ObjectName = stat.ObjectName;
                fm.Etag = stat.ETag;
                return fm;
            }
            catch (BucketNotFoundException) {
                return null;
            }
            catch (ObjectNotFoundException) {
                return null;
            }
        }

    }
}
