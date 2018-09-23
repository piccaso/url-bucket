using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UrlBucket.Lib.Models;
using UrlBucket.Lib.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace UrlBucket.Controllers {

    [Route("api"), ApiController]
    public class ApiController : ControllerBase {
        private readonly StorageService _storageService;
        private readonly DownloadService _downloadService;
        private readonly ILogger _logger;

        public ApiController(StorageService storageService, DownloadService downloadService, ILogger<ApiController> logger) {
            _storageService = storageService;
            _downloadService = downloadService;
            _logger = logger;
        }

        private ProblemDetails Sucess(int status = StatusCodes.Status200OK) => new ProblemDetails {Status = status, Title = "Sucess"};

        private BadRequestObjectResult Exception(Exception e) {
            _logger.LogError(e, "");
            var pd = new ProblemDetails {
                Status = 400,
                Title = e.Message,
                Detail = e.GetType().FullName,
            };
            try {
                var frame = new StackTrace(e, true).GetFrame(0);
                var details = $" Method:{frame.GetMethod()} Line:{frame.GetFileLineNumber()} File:{frame.GetFileName()}";
            }
            catch {
                // no stacktrace available (release build without pdb...)
            }
            return BadRequest(pd);
        }

        private NotFoundObjectResult FileNotFound() => NotFound(new ProblemDetails{Status = 404, Title = "Not found"});

        /// <summary>
        /// Stores an asset after downloading it from the given URL.
        /// </summary>
        /// <param name="url">The URL where the asset is currently located</param>
        /// <param name="userAgent">Optional 'user-agent' header to use when downloading</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("store-url")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProblemDetails>> Get([Required] Uri url, string userAgent = null, CancellationToken ct = default(CancellationToken)) {
            try {
                var file = await _downloadService.DownloadUrlAsync(url, userAgent);
                await _storageService.UploadFileAsync(file, ct);
                return Sucess();
            }
            catch (Exception e) {
                return Exception(e);
            }
        }

        /// <summary>
        /// Retrieves a previously stored asset as an easy to consume model.
        /// </summary>
        /// <param name="url">The URL identifying the asset</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("retrieve")]
        [ProducesResponseType(typeof(DownloadFileModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DownloadFileModel>> RetrieveFile([Required] Uri url, CancellationToken ct = default(CancellationToken)) {
            
            try {
                var file = await _storageService.DownloadFileAsync(url, ct);
                if (file is null) return FileNotFound();
                return Ok(file);
            }
            catch (Exception e) {
                return Exception(e);
            }
        }

        /// <summary>
        /// Stores an asset (without downloading it).
        /// </summary>
        /// <param name="content">Content of the asset</param>
        /// <param name="url">URL identifying the asset</param>
        /// <param name="contentType">Content type of the asset</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpPost("store-bytes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProblemDetails>> StoreBytes([FromBody,Required] byte[] content, [Required] Uri url, string contentType ,CancellationToken ct = default(CancellationToken)) {
            try {
                var file = new UploadFileModel {
                    Content = content,
                    Url = url,
                    ContentType = contentType,
                };
                await _storageService.UploadFileAsync(file, ct);
                return Sucess();
            }
            catch (Exception e) {
                return Exception(e);
            }
        }

        /// <summary>
        /// Retrieves a previously stored asset as HTTP Response.
        /// </summary>
        /// <param name="url">The URL identifying the asset</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        [HttpGet("retrieve-bytes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<byte[]>> RetrieveBytes([Required] Uri url, CancellationToken ct = default(CancellationToken)) {
            try {
                var file = await _storageService.DownloadFileAsync(url, ct);
                if (file is null) return FileNotFound();
                return File(file.Content, file.ContentType);
            }
            catch (Exception e) {
                return Exception(e);
            }
        }

        [HttpGet("/"),ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index() {
            return Redirect("~/swagger");
        }
    }
}
