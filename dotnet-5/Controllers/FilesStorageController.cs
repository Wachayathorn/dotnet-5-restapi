using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace dotnet_5.Controllers
{
    [Route("file")]
    [RequestSizeLimit(5368706371)]
    [RequestFormLimits(MultipartBodyLengthLimit = 5368706371)]
    public class FilesStorageController : Controller
    {
        private readonly IOptions<EnvConfiguration> env;

        public FilesStorageController(IOptions<EnvConfiguration> env)
        {
            this.env = env;
        }

        [HttpPost , Route("upload")]
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

                using (FileStream stream = new FileStream(filePath , FileMode.Create))
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
    }
}
