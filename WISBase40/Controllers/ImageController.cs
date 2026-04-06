using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Net.Mime;
using WIS.Configuration;
using WIS.WebApplication.Models;

namespace WIS.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : Controller
    {
        private readonly IOptions<ApplicationSettings> _appSettings;

        public ImageController(IOptions<ApplicationSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        [HttpGet("[action]")]
        public IActionResult Get(string id)
        {
            var path = _appSettings.Value.Images.FirstOrDefault(i => i.Id == id)?.Path ?? "";
            var bytes = new byte[0];

            new FileExtensionContentTypeProvider().TryGetContentType(path, out string contentType);

            if (System.IO.File.Exists(path))
                bytes = System.IO.File.ReadAllBytes(path);

            return File(bytes, contentType);
        }

        [HttpGet("[action]")]
        public IActionResult GetFaviconName()
        {
            return Ok(_appSettings.Value.FaviconName ?? "favicon.ico");
        }
    }
}
