using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace BackApp.Services.S3.Client
{
    public class S3Client : IS3Client
    {
        IAmazonS3 _s3Client;

        IAmazonS3 S3ClientInstance => _s3Client ??= new AmazonS3Client();

        public S3Client()
        {
            AWSConfigsS3.UseSignatureVersion4 = true;
        }

        public async Task<byte[]> GetObject(string pbucketName, string pkey)
        {
            var response = await S3ClientInstance.GetObjectAsync(pbucketName, pkey);
            using (var ms = new MemoryStream())
            {
                response.ResponseStream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public async Task<string> GetObjectToString(string pbucketName, string pkey)
        {
            var resp = await S3ClientInstance.GetObjectAsync(pbucketName, pkey);
            string s3Contents;
            using (var stream = resp.ResponseStream)
            {
                TextReader tr = new StreamReader(stream);
                s3Contents = tr.ReadToEnd();
            }
            return s3Contents;
        }

        public string GetObjectPreSignedUrl(string pbucketName, string pkey, short expirationTime = 30, bool isPutObject = false)
        {
            GetPreSignedUrlRequest request1;
            if (!isPutObject)
            {
                request1 = new GetPreSignedUrlRequest
                {
                    BucketName = pbucketName,
                    Key = pkey,
                    Expires = DateTime.Now.AddMinutes(expirationTime)
                };
            }
            else
            {
                request1 = new GetPreSignedUrlRequest
                {
                    BucketName = pbucketName,
                    Key = pkey,
                    Verb = HttpVerb.PUT,
                    Expires = DateTime.Now.AddMinutes(expirationTime)
                };
                Console.WriteLine("Url is in Verb PUT");

            }
            var urlString = S3ClientInstance.GetPreSignedURL(request1);
            return urlString;
        }

        public string GetObjectUrl(string pbucketName, string pkey)
        {
            return string.Format(@"https://{0}.s3.amazonaws.com/{1}", pbucketName, pkey);
        }

        public async Task<string[]> ListObjects(string pbucketName, string pkeyPrefix = "")
        {
            List<string> keys = new List<string>();
            ListObjectsV2Request request = new ListObjectsV2Request
            {
                BucketName = pbucketName,
                MaxKeys = 10
            };
            if (!string.IsNullOrEmpty(pkeyPrefix))
            {
                request.Prefix = pkeyPrefix;
            }

            ListObjectsV2Response response;
            do
            {
                response = await S3ClientInstance.ListObjectsV2Async(request);
                // Process the response.
                foreach (S3Object entry in response.S3Objects)
                {
                    Console.WriteLine("key = {0} size = {1}",
                        entry.Key, entry.Size);

                    keys.Add(entry.Key);
                }
                request.ContinuationToken = response.NextContinuationToken;
            } while (response.IsTruncated);

            return keys.ToArray();
        }

        public async Task<bool> PutObject(string pbucketName, string pkey, byte[] bytes, string contentType, List<Tag> tags = null)
        {
            try
            {
                var d = await S3ClientInstance.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = pbucketName,
                    Key = pkey,
                    InputStream = new MemoryStream(bytes),
                    ContentType = contentType,
                    TagSet = tags
                });

                return d.HttpStatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving object to S3 bucket '{pbucketName}', key '{pkey}': {ex.Message}", ex);
            }
        }

        public async Task<bool> PutObjectHeadersAttachment(string pbucketName, string pkey, MemoryStream stream, string name, string contentType = "", List<Tag> tags = null)
        {
            try
            {
                var d = await S3ClientInstance.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = pbucketName,
                    Key = pkey,
                    InputStream = stream,
                    ContentType = contentType,
                    Headers = { ContentDisposition = $"attachment; filename={name}" },
                    TagSet = tags
                });
                return d.HttpStatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving object to S3 bucket '{pbucketName}', key '{pkey}': {ex.Message}", ex);
            }
        }

        public async Task<bool> PutObject(byte[] pdata, string pbucketName, string pkey, List<Tag> tags = null)
        {
            var fileTransferUtility = new TransferUtility(S3ClientInstance);
            using (var fileToUpload = new MemoryStream(pdata))
            {
                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = pbucketName,
                    InputStream = fileToUpload,
                    Key = pkey,
                    TagSet = tags
                };

                await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
            }
            return true;
        }

        public async Task<bool> PutPublicObject(byte[] pdata, string pbucketName, string pkey, List<Tag> tags = null)
        {
            var fileTransferUtility = new TransferUtility(S3ClientInstance);
            using (var fileToUpload = new MemoryStream(pdata))
            {
                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = pbucketName,
                    InputStream = fileToUpload,
                    Key = pkey,
                    CannedACL = S3CannedACL.PublicRead,
                    TagSet = tags
                };
                await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);
            }
            return true;
        }

        public async Task<bool> RemoveObject(string pbucketName, string pkey)
        {
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = pbucketName,
                Key = pkey
            };

            var response = await S3ClientInstance.DeleteObjectAsync(deleteObjectRequest);

            return response.HttpStatusCode == HttpStatusCode.OK;
        }

        public async Task<bool> ExistsKey(string pbucketName, string pkey)
        {
            try
            {
                await S3ClientInstance.GetObjectMetadataAsync(pbucketName, pkey);
                return true;
            }
            catch (AggregateException ex)
            {
                Console.WriteLine($"No existe la key {pkey}");
                Console.WriteLine(ex.Message);
                return false;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"No existe el doc bucket:{pbucketName}, key {pkey}");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }
    }
}
