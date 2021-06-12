using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_5.Controllers
{
    [Route("file")]
    public class FilesStorageController : Controller
    {
        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 1073741824)]
        [RequestSizeLimit(1073741824)]
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
