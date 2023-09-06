namespace ImagesFromLocal
{
    public class RequestDownload
    {
        public IEnumerable<string> ImageUrls { get; set; }
        public int MaxDownloadAtOnce { get; set; }
    }

    public class ResponseDownload
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public IDictionary<string, string> UrlAndNames { get; set; }
    }

    public class DownloadManager
    {
        public int currentDownloadNo;
        public string filePath;
        public int imgUrlCount;
        public RequestDownload reqestDownload;
        public ResponseDownload responseDownload;

        public async Task<ResponseDownload> downloadImages()
        {

            if (currentDownloadNo < imgUrlCount)
            {
                List<Byte[]> bytes = new List<Byte[]>();

                int downloadUpTo = (currentDownloadNo + reqestDownload.MaxDownloadAtOnce);
                bool willDownloadFinish = false;
                if (downloadUpTo >= imgUrlCount)
                {
                    downloadUpTo = imgUrlCount;
                    willDownloadFinish = true;
                }

                //downloadUpTo = downloadUpTo >= (currentDM.imgUrlCount-1) ?currentDM.imgUrlCount-1 :downloadUpTo;
                try
                {
                    while (currentDownloadNo < downloadUpTo)
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            HttpResponseMessage response = await client.GetAsync(reqestDownload.ImageUrls.ElementAt(currentDownloadNo));
                            bytes.Add(await response.Content.ReadAsByteArrayAsync());
                            //return File(content, "image/png", parammodel.modelname);
                        }
                        currentDownloadNo++;
                    }
                    saveImagesToFolder(bytes);
                    if (!willDownloadFinish)
                        await downloadImages();
                    else
                        return responseDownload;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    responseDownload.Success = false;
                    responseDownload.Message = ex.Message;
                }
            }
            return responseDownload;
        }
        public void saveImagesToFolder(List<Byte[]> bytes)
        {
            foreach (Byte[] imageData in bytes)
            {
                string imageName = string.Format(@"image_{0}.jpg", Guid.NewGuid());
                string imagePath = Path.Combine(filePath, imageName);
                System.IO.File.WriteAllBytes(imagePath, imageData);
                responseDownload.UrlAndNames.Add(imageName, Convert.ToBase64String(imageData));
            }
        }
    }

}
