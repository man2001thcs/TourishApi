using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace WebApplication1.Controllers.Image
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageUploadController : ControllerBase
    {
        [HttpPost, DisableRequestSizeLimit]
        public IActionResult Upload([FromForm] String productId)
        {
            try
            {
                var files = Request.Form.Files;
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (files.Any(f => f.Length == 0))
                {
                    return BadRequest();
                }

                int index = 1;
                foreach (var file in files)
                {
                    var fileNameOld = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                    FileInfo fi = new FileInfo(fileNameOld);
                    string ext = fi.Extension;
                    var fileName = productId + "_" + index + "." + ext;
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName); //you can add this path to a list and then return all dbPaths to the client if require
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
                return Ok("All the files are successfully uploaded.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}
