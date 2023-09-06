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
        public int currentDownloadNo;
        public ImageDownloadController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost]
        public async Task<ResponseDownload> Post(RequestDownload reqDownLoad)
        {
            ResponseDownload responseDownload = new ResponseDownload() {
                Success = false,
                Message = "",
                UrlAndNames = new Dictionary<string, string>()
            };
            try
            {
                //HttpClient client = new HttpClient();
                if (!Directory.Exists(_environment.ContentRootPath+ "\\Uploads\\"))
                {
                    Directory.CreateDirectory(_environment.ContentRootPath + "\\Uploads\\");
                }

                int count = 0;
                foreach (var item in reqDownLoad.ImageUrls)
                {
                    /*HttpWebRequest request = (HttpWebRequest)WebRequest.Create(item);
                    request.Method = "GET";
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream stream = response.GetResponseStream();
                    byte[] imageData = new byte[response.ContentLength];
                    //Image image = Image.FromStream(new MemoryStream(imageData));
                    //File.WriteAllBytes(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "image1.jpg"), imageData);
                    string fileName = Path.Combine((_environment.WebRootPath + "\\Upload\\"), "image1.jpg");
                    System.IO.File.WriteAllBytes(fileName, imageData);

                    responseDownload.UrlAndNames[item] = fileName;*/
                    Console.WriteLine("url path -> "+item);

                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync(item);
                        byte[] imageData = await response.Content.ReadAsByteArrayAsync();
                        string fileName = Path.Combine((_environment.ContentRootPath + "\\Uploads\\"), "image"+ count.ToString() +".jpg");
                        System.IO.File.WriteAllBytes(fileName, imageData);
                        count++;
                        //return File(content, "image/png", parammodel.modelname);
                    }
                }
                
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                responseDownload.Success = false;
            }

            return responseDownload;
            
        }
    }
}
