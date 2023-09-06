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
            currentDM = new DownloadManager()
            {
                currentDownloadNo = 0,
                reqestDownload = reqDownLoad,
                imgUrlCount = reqDownLoad.ImageUrls.Count(),
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")// string.IsNullOrWhiteSpace(_environment.WebRootPath)? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"):Path.Combine(_environment.WebRootPath, "\\Downloads\\"),
            };
            
            
            /*if (!Directory.Exists(currentDM.filePath))
            {
                Directory.CreateDirectory(currentDM.filePath);
            }*/

            currentDM.responseDownload = new ResponseDownload()
            {
                Success = true,
                UrlAndNames = new Dictionary<string, string>(),
            };

            await currentDM.downloadImages();

            return currentDM.responseDownload;
        }

        /*private async void downloadImages()
        {
            
            if (currentDM != null && currentDM.currentDownloadNo < currentDM.imgUrlCount)
            {
                List<Byte[]> bytes = new List<Byte[]>();
                
                int downloadUpTo = (currentDM.currentDownloadNo + currentDM.reqestDownload.MaxDownloadAtOnce);
                bool willDownloadFinish = false;
                if(downloadUpTo >= (currentDM.imgUrlCount))
                {
                    downloadUpTo = currentDM.imgUrlCount;
                    willDownloadFinish = true;
                }
                
                //downloadUpTo = downloadUpTo >= (currentDM.imgUrlCount-1) ?currentDM.imgUrlCount-1 :downloadUpTo;
                try
                {
                    while (currentDM.currentDownloadNo < downloadUpTo)
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            HttpResponseMessage response = await client.GetAsync(currentDM.reqestDownload.ImageUrls.ElementAt(currentDM.currentDownloadNo));
                            bytes.Add(await response.Content.ReadAsByteArrayAsync());
                            //return File(content, "image/png", parammodel.modelname);
                        }
                        currentDM.currentDownloadNo++;
                    }
                    saveImagesToFolder(bytes);
                    if(!willDownloadFinish)
                        downloadImages();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    currentDM.responseDownload.Success = false;
                    currentDM.responseDownload.Message = ex.Message;
                }
            }

        }*/

        /*public void saveImagesToFolder(List<Byte[]> bytes)
        {
            foreach (Byte[] imageData in bytes)
            {
                string imageName = string.Format(@"image_{0}.jpg", Guid.NewGuid());
                string imagePath = Path.Combine(currentDM.filePath, imageName);
                System.IO.File.WriteAllBytes(imagePath, imageData);
                currentDM.responseDownload.UrlAndNames.Add(imageName, "");
            }
        }*/

            /*[HttpPost]
            public async Task<ResponseDownload> Post(RequestDownload reqDownLoad)
            {
                ResponseDownload responseDownload = new ResponseDownload() {
                    Success = true,
                    Message = "",
                    UrlAndNames = new Dictionary<string, string>()
                };
                try
                {
                    string folderName = "Downloads";
                    //HttpClient client = new HttpClient();
                    if (!Directory.Exists(_environment.ContentRootPath+ "\\"+ folderName + "\\"))
                    {
                        Directory.CreateDirectory(_environment.ContentRootPath + "\\" + folderName + "\\");
                    }

                    foreach (var item in reqDownLoad.ImageUrls)
                    {
                        //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(item);
                        //request.Method = "GET";
                        //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        //Stream stream = response.GetResponseStream();
                        //byte[] imageData = new byte[response.ContentLength];
                        //Image image = Image.FromStream(new MemoryStream(imageData));
                        //File.WriteAllBytes(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "image1.jpg"), imageData);
                        //string fileName = Path.Combine((_environment.WebRootPath + "\\Upload\\"), "image1.jpg");
                        //System.IO.File.WriteAllBytes(fileName, imageData);

                        //responseDownload.UrlAndNames[item] = fileName;


                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync(item);
                        byte[] imageData = await response.Content.ReadAsByteArrayAsync();
                        string fileName = Path.Combine((_environment.ContentRootPath + "\\" + folderName + "\\"), string.Format(@"image_{0}.jpg", Guid.NewGuid()));
                        System.IO.File.WriteAllBytes(fileName, imageData);
                        responseDownload.UrlAndNames.Add(item,fileName);
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
            
        }*/
    }
}
