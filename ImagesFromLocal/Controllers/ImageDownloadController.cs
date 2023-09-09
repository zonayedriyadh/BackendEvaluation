using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using System;
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
            reqDownLoad.MaxDownloadAtOnce = reqDownLoad.MaxDownloadAtOnce == 0 ? 1 : Math.Abs(reqDownLoad.MaxDownloadAtOnce);
            currentDM = new DownloadManager()
            {
                currentDownloadNo = 0,
                requestDownload = reqDownLoad,
                imageUrlsCount = reqDownLoad.ImageUrls.Count(),
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "Downloads")
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

            await currentDM.DownloadImages();
            return currentDM.responseDownload;
        }

        [Route("get-Image-by-name/{image_name}")]
        [HttpGet]
        public string GetImageByName(string image_name)
        {
            var path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Downloads", image_name);
            if (System.IO.File.Exists(path))
            {
                byte[] image = System.IO.File.ReadAllBytes(path);
                return Convert.ToBase64String(image);
            }
            return "Image not found";
        }
    }
}
