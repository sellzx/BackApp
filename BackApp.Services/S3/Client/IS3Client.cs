using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3.Model;

namespace BackApp.Services.S3.Client
{
    public interface IS3Client
    {
        Task<bool> PutObject(byte[] pdata, string pbucketName, string pkey, List<Tag> tags = null);

        Task<bool> PutObject(string pbucketName, string pkey, byte[] bytes, string contentType, List<Tag> tags = null);

        Task<bool> PutObjectHeadersAttachment(string pbucketName, string pkey, MemoryStream stream, string name, string contentType = "", List<Tag> tags = null);

        Task<bool> PutPublicObject(byte[] pdata, string pbucketName, string pkey, List<Tag> tags = null);

        Task<byte[]> GetObject(string pbucketName, string pkey);

        Task<string> GetObjectToString(string pbucketName, string pkey);

        Task<string[]> ListObjects(string pbucketName, string pkeyPrefix = "");

        Task<bool> RemoveObject(string pbucketName, string pkey);

        string GetObjectUrl(string pbucketName, string pkey);

        string GetObjectPreSignedUrl(string pbucketName, string pkey, short expirationTime = 5, bool isPutObject = false);

        Task<bool> ExistsKey(string pbucketName, string pkey);
    }
}
