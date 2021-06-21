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

namespace dotnet_5.Controllers
{
    [Route("file")]
    [RequestSizeLimit(5368706371)]
    [RequestFormLimits(MultipartBodyLengthLimit = 5368706371)]
    public class FilesStorageController : Controller
    {
        private readonly IOptions<EnvConfiguration> env;
        private static readonly FormOptions _defaultFormOptions = new FormOptions();
        private readonly string[] _permittedExtensions = { ".jpeg", ".mp4", ".mov" , ".jpg" };

        public FilesStorageController(IOptions<EnvConfiguration> env)
        {
            this.env = env;
        }

        [HttpPost, Route("upload-large-file")]
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

        [HttpPost, Route("upload-small-file")]
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

    }
}
