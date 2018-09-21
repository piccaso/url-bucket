using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UrlBucket.Lib.Models;
using UrlBucket.Lib.Services;
using Microsoft.AspNetCore.Http;

namespace UrlBucket.Controllers {
    [Route("api")]
    [ApiController]
    public class ApiController : ControllerBase {
        private readonly StorageService _storageService;
        private readonly DownloadService _downloadService;

        public ApiController(StorageService storageService, DownloadService downloadService) {
            _storageService = storageService;
            _downloadService = downloadService;
        }

        [HttpGet("store-url")]
        [ProducesResponseType(typeof(FileModel),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FileModel>> StoreUrl([Required] Uri url, string userAgent = null, CancellationToken ct = default(CancellationToken)) {

            try {
                var file = await _downloadService.DownloadUrlAsync(url, userAgent);
                await _storageService.UploadFileAsync(file, ct);
                return Ok(file);
            }
            catch (Exception e) {
                return BadRequest(new ProblemDetails() {
                    Status = 400,
                    Detail = e.Message,
                    Title = e.GetType().FullName,
                });
            }
        }

        [HttpPost("store-file")]
        [ProducesResponseType(typeof(FileModel),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FileModel>> StoreFile([FromBody] UploadFileModel file, CancellationToken ct = default(CancellationToken)) {
            try {
                await _storageService.UploadFileAsync(file, ct);
                return Ok(file);
            }
            catch (Exception e) {
                return BadRequest(new ProblemDetails() {
                    Status = 400,
                    Detail = e.Message,
                    Title = e.GetType().FullName,
                });
            }
        }


        [HttpGet("retrieve-file")]
        [ProducesResponseType(typeof(DownloadFileModel),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DownloadFileModel>> RetrieveFile(Uri url, CancellationToken ct = default(CancellationToken)) {
            
            try {
                var file = await _storageService.DownloadFileAsync(url, ct);
                if (file is null) return NotFound();
                return Ok(file);
            }
            catch (Exception e) {
                return BadRequest(new ProblemDetails() {
                    Status = 400,
                    Detail = e.Message,
                    Title = e.GetType().FullName,
                });
            }
        }

        [HttpGet("retrieve-bytes")]
        [ProducesResponseType(typeof(byte[]),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DownloadFileModel>> RetrieveBytes(Uri url, CancellationToken ct = default(CancellationToken)) {

            try {
                var file = await _storageService.DownloadFileAsync(url, ct);
                if (file is null) return NotFound();
                return File(file.Content, file.ContentType);
            }
            catch (Exception e) {
                return BadRequest(new ProblemDetails() {
                    Status = 400,
                    Detail = e.Message,
                    Title = e.GetType().FullName,
                });
            }
        }
    }
}
