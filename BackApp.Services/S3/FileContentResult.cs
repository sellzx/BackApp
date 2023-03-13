using System.IO;

namespace BackApp.Services.S3
{
    public class FileContentResult
    {
        private MemoryStream memoryStream;
        private string contentType;
        private string v;

        public FileContentResult(MemoryStream memoryStream, string contentType, string v)
        {
            this.memoryStream = memoryStream;
            this.contentType = contentType;
            this.v = v;
        }
    }
}