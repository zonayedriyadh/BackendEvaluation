﻿namespace ImagesFromLocal
{
    public class RequestDownload
    {
        public IEnumerable<string> ImageUrls { get; set; }
        public int MaxDownloadAtOnce { get; set; }
    }
}
