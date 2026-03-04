using Business;
using Business.Interfaces;
using DataTransferObject;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using UnitOfWork;

namespace Website
{
    [DisableRequestSizeLimit]
    public class UploadController : Controller
    {
        private IParametrosBL _paramBL { get; set; }

        public UploadController(IParametrosBL paramBL)
        {
            _paramBL= paramBL;
        }
        // Single file upload
        [HttpPost("upload/single")]
        [RequestFormLimits(MultipartBodyLengthLimit = 536870912)]   // 500 MB
        [RequestSizeLimit(536870912)] // 500 MB
        public IActionResult Single(IFormFile file)
        {
            try
            {
                string path = _paramBL.GetParametroCharAsync("Path.Upload.Files").Result;
                DeleteOldFiles(path);

                var name = file.FileName;
                string filePath = Path.Combine(path, name);
                if (!System.IO.File.Exists(filePath))
                {
                    using (var stream = new FileStream(filePath, FileMode.Create,FileAccess.Write, FileShare.Write))
                    {
                        // Save the file
                        file.CopyTo(stream);
                    }
                }
                

                // Put your code here
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Multiple files upload
        [HttpPost("upload/multiple")]
        [RequestFormLimits(MultipartBodyLengthLimit = 536870912)]   // 500 MB
        [RequestSizeLimit(536870912)] // 500 MB
        public IActionResult Multiple(IFormFile[] files)
        {
            try
            {
                var uof = new UnitOfWork.UnitOfWorkFactory();
                ParametrosBL _paramBL = new ParametrosBL(uof);
                string path = _paramBL.GetParametroCharAsync("Path.Upload.Files").Result;
                DeleteOldFiles(path);

                // Put your code here
                foreach (var file in files)
                {
                    var name = file.FileName;

                    using (var stream = new FileStream(Path.Combine(path, name), FileMode.Create))
                    {
                        // Save the file
                        file.CopyTo(stream);
                    }
                }
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Multiple files upload with parameter
        [HttpPost("upload/{id}")]
        [RequestFormLimits(MultipartBodyLengthLimit = 536870912)]   // 500 MB
        [RequestSizeLimit(536870912)] // 500 MB
        public IActionResult Post(IFormFile[] files, int id)
        {
            try
            {
                // Put your code here
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // Image file upload (used by HtmlEditor components)
        [HttpPost("upload/image")]
        [RequestFormLimits(MultipartBodyLengthLimit = 536870912)]   // 500 MB
        [RequestSizeLimit(536870912)] // 500 MB
        public IActionResult Image(IFormFile file)
        {
            try
            {
                //var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                //using (var stream = new FileStream(Path.Combine(environment.WebRootPath, fileName), FileMode.Create))
                //{
                //    // Save the file
                //    file.CopyTo(stream);

                //    // Return the URL of the file
                //    var url = Url.Content($"~/{fileName}");

                //    return Ok(new { Url = url });
                //}

                // Put your code here
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private void DeleteOldFiles(string path)
        {
            //Elimina los archivos con mas de 3 días para mantener el directorio limpio.
            string[] lstArchs = Directory.GetFiles(path);
            foreach (string arch in lstArchs)
            {
                DateTime fechaCreacion = System.IO.File.GetCreationTime(arch);
                if (fechaCreacion < DateTime.Now.AddDays(-3))
                    System.IO.File.Delete(arch);
            }
        }

    }


}
