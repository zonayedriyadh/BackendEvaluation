namespace ImagesFromLocal
{
    public class DownloadManager
    {
        public int currentDownloadNo;
        public string filePath;
        public int imgUrlCount;
        public RequestDownload reqestDownload;
        public ResponseDownload responseDownload;

        public struct NameAndUrl
        {
            public Byte[] bytes;
            public string url;
        }

        public async Task<ResponseDownload> downloadImages()
        {

            if (currentDownloadNo < imgUrlCount)
            {
                List<NameAndUrl> bytesAndUrl = new List<NameAndUrl>();

                int downloadUpTo = (currentDownloadNo + reqestDownload.MaxDownloadAtOnce);
                bool willDownloadFinish = false;
                if (downloadUpTo >= imgUrlCount)
                {
                    downloadUpTo = imgUrlCount;
                    willDownloadFinish = true;
                }
                try
                {
                    while (currentDownloadNo < downloadUpTo)
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            HttpResponseMessage response = await client.GetAsync(reqestDownload.ImageUrls.ElementAt(currentDownloadNo));
                            if(response.IsSuccessStatusCode)
                            {
                                Byte[] imageData = await response.Content.ReadAsByteArrayAsync();
                                bytesAndUrl.Add(new NameAndUrl() { bytes = imageData, url = reqestDownload.ImageUrls.ElementAt(currentDownloadNo) });
                            }
                            else
                            {
                                responseDownload.Message += " Download failed at " + reqestDownload.ImageUrls.ElementAt(currentDownloadNo);
                            }
                            
                            //return File(content, "image/png", parammodel.modelname);
                        }
                        currentDownloadNo++;
                    }
                    SaveImagesToFolder(bytesAndUrl);
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
        public void SaveImagesToFolder(List<NameAndUrl> downloadedImages)
        {
            foreach (NameAndUrl imageData in downloadedImages)
            {
                string imageName = string.Format(@"image_{0}.jpg", Guid.NewGuid());
                string imagePath = Path.Combine(filePath, imageName);
                System.IO.File.WriteAllBytes(imagePath, imageData.bytes);
                responseDownload.UrlAndNames.Add(imageName, imageData.url);
            }
        }
    }
}
