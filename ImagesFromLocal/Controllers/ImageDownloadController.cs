using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using System.Net;
using System.Drawing;
using Microsoft.AspNetCore.Hosting.Server;
using System.Diagnostics.Metrics;

namespace ImagesFromLocal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageDownloadController : ControllerBase
    {
        public static IWebHostEnvironment _environment;
        public DownloadManager? currentDM;
        public ImageDownloadController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost]
        public async Task<ResponseDownload> Post(RequestDownload reqDownLoad)
        {
            reqDownLoad.MaxDownloadAtOnce = reqDownLoad.MaxDownloadAtOnce <= 0 ? 1 : reqDownLoad.MaxDownloadAtOnce;
            currentDM = new DownloadManager()
            {
                currentDownloadNo = 0,
                reqestDownload = reqDownLoad,
                imgUrlCount = reqDownLoad.ImageUrls.Count(),
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")// string.IsNullOrWhiteSpace(_environment.WebRootPath)? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"):Path.Combine(_environment.WebRootPath, "\\Downloads\\"),
            };
            
            
            if (!Directory.Exists(currentDM.filePath))
            {
                Directory.CreateDirectory(currentDM.filePath);
            }

            currentDM.responseDownload = new ResponseDownload()
            {
                Success = true,
                UrlAndNames = new Dictionary<string, string>(),
            };

            await currentDM.downloadImages();
            return currentDM.responseDownload;
        }

        [Route("get-Image-by-name/{image_name}")]
        [HttpGet]
        public string GetImageByName(string image_name)
        {
            if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + image_name)))
            {
                byte[] image = System.IO.File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/" + image_name));
                return Convert.ToBase64String(image);
            }
            return "Image not found";
        }
    }
}
