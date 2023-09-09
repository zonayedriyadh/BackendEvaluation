namespace ImagesFromLocal
{
    public class DownloadManager
    {
        public int currentDownloadNo;
        public string filePath;
        public int imageUrlsCount;
        public RequestDownload requestDownload;
        public ResponseDownload responseDownload;

        public struct ImageDataAndUrl
        {
            public Byte[] bytes;
            public string url;
        }

        public async Task<ResponseDownload> DownloadImages()
        {
            if (currentDownloadNo < imageUrlsCount)
            {
                List<ImageDataAndUrl> imageDataAndUrl = new List<ImageDataAndUrl>();

                int downloadUpTo = (currentDownloadNo + requestDownload.MaxDownloadAtOnce);
                bool isAllImagesCovered = false;
                if (downloadUpTo >= imageUrlsCount)
                {
                    downloadUpTo = imageUrlsCount;
                    isAllImagesCovered = true;
                }
                try
                {
                    while (currentDownloadNo < downloadUpTo)
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            HttpResponseMessage response = await client.GetAsync(requestDownload.ImageUrls.ElementAt(currentDownloadNo));
                            if(response.IsSuccessStatusCode)
                            {
                                Byte[] imageData = await response.Content.ReadAsByteArrayAsync();
                                imageDataAndUrl.Add(new ImageDataAndUrl() { bytes = imageData, url = requestDownload.ImageUrls.ElementAt(currentDownloadNo) });
                            }
                            else
                            {
                                responseDownload.Message += " Download failed at " + requestDownload.ImageUrls.ElementAt(currentDownloadNo);
                            }                            
                        }
                        currentDownloadNo++;
                    }
                    SaveImagesToFolder(imageDataAndUrl);
                    if (!isAllImagesCovered)
                        await DownloadImages();
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
        public void SaveImagesToFolder(List<ImageDataAndUrl> downloadedImages)
        {
            foreach (ImageDataAndUrl imageData in downloadedImages)
            {
                string imageName = string.Format(@"image_{0}.png", Guid.NewGuid());
                string imagePath = Path.Combine(filePath, imageName);
                System.IO.File.WriteAllBytes(imagePath, imageData.bytes);
                responseDownload.UrlAndNames.Add(imageName, imageData.url);
            }
        }
    }
}
