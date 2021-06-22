using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace dotnet_5.Controllers
{
    [Route("file")]
    [RequestSizeLimit(5368706371)]
    [RequestFormLimits(MultipartBodyLengthLimit = 5368706371)]
    public class FilesStorageController : Controller
    {
        private readonly IOptions<EnvConfiguration> env;
        private static readonly FormOptions _defaultFormOptions = new FormOptions();
        private readonly string[] _permittedExtensions = { ".jpeg", ".mp4", ".mov", ".jpg" };

        public FilesStorageController(IOptions<EnvConfiguration> env)
        {
            this.env = env;
        }

        [HttpPost, Route("upload-with-formfile")]
        public async Task<ActionResult> Upload(IFormFile file)
        {
            try
            {
                string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage");

                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }

                var fileName = file.FileName;
                var filePath = Path.Combine(FilePath, fileName);

                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex);
            }
        }

        [HttpPost, Route("upload-with-stream")]
        [DisableFormValueModelBinding]
        public async Task<IActionResult> UploadPhysical()
        {
            if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
            {
                ModelState.AddModelError("File", $"The request couldn't be processed (Error 1).");
                return BadRequest(ModelState);
            }

            var boundary = MultipartRequestHelper.GetBoundary(MediaTypeHeaderValue.Parse(Request.ContentType), _defaultFormOptions.MultipartBoundaryLengthLimit);
            var reader = new MultipartReader((string)boundary, HttpContext.Request.Body);
            var section = await reader.ReadNextSectionAsync();

            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);

                if (hasContentDispositionHeader)
                {
                    if (!MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                    {
                        ModelState.AddModelError("File", $"The request couldn't be processed (Error 2).");
                        return BadRequest(ModelState);
                    }
                    else
                    {
                        var streamedFileContent = await FileHelpers.ProcessStreamedFile(section, contentDisposition, ModelState, _permittedExtensions, env.Value.UPLOAD_FILE_SIZE);

                        if (!ModelState.IsValid)
                        {
                            return BadRequest(ModelState);
                        }

                        string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage");

                        if (!Directory.Exists(FilePath))
                        {
                            Directory.CreateDirectory(FilePath);
                        }

                        var filePath = Path.Combine(FilePath, contentDisposition.FileName.Value);
                        using (var targetStream = System.IO.File.Create(filePath))
                        {
                            await targetStream.WriteAsync(streamedFileContent);
                        }
                    }
                }

                section = await reader.ReadNextSectionAsync();
            }

            return Created(nameof(FilesStorageController), null);
        }

        [HttpPost, Route("upload-with-chunked")]
        public ActionResult UploadChunk(IFormFile file, string chunkMetadata)
        {
            try
            {
                var metaDataObject = JsonConvert.DeserializeObject<ChunkMetadata>(chunkMetadata);
                if (!string.IsNullOrEmpty(chunkMetadata))
                {
                    CheckFileExtensionValid(metaDataObject.FileName);
                    string tempPath = Path.Combine(Directory.GetCurrentDirectory(), "temp");
                    var tempFilePath = Path.Combine(tempPath, metaDataObject.FileGuid + ".tmp");
                    if (!Directory.Exists(tempPath))
                        Directory.CreateDirectory(tempPath);

                    AppendContentToFile(tempFilePath, file);

                    if (metaDataObject.Index == (metaDataObject.TotalCount - 1))
                        ProcessUploadedFile(tempFilePath, metaDataObject.FileName);
                }

            }
            catch
            {
                return BadRequest();
            }
            return Ok();
        }
        public void CheckFileExtensionValid(string fileName)
        {
            fileName = fileName.ToLower();
            string[] imageExtensions = { ".jpg", ".jpeg", ".gif", ".png", ".mp4", ".mov" };

            var isValidExtenstion = imageExtensions.Any(ext =>
            {
                return fileName.LastIndexOf(ext) > -1;
            });
            if (!isValidExtenstion)
                throw new Exception("Not allowed file extension");
        }
        public void ProcessUploadedFile(string tempFilePath, string fileName)
        {
            string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage");

            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }

            var filePath = Path.Combine(FilePath, fileName);
            System.IO.File.Copy(tempFilePath, Path.Combine(FilePath, fileName));
        }
        public void AppendContentToFile(string path, IFormFile content)
        {
            using (var stream = new FileStream(path, FileMode.Append, FileAccess.Write))
            {
                content.CopyTo(stream);
                CheckMaxFileSize(stream);
            }
        }
        public void CheckMaxFileSize(FileStream stream)
        {
            if (stream.Length > 5368706371)
                throw new Exception("File is too large");
        }
    }
}
