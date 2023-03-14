using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using BackApp.Models.AWS.DynamoDBEntities;
using BackApp.Models.Input;
using BackApp.Services.DynamoDB;
using BackApp.Services.S3.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BackApp.Services.S3
{
    public class S3Service : S3Client
    {
        public async Task<bool> DataTransferAsync(byte[] imageBytes, string owner)
        {
            var client = new AmazonS3Client("", "", RegionEndpoint.USEast1);


            try
            {
                var stream = new MemoryStream(imageBytes);
                stream.Seek(0, SeekOrigin.Begin);
                var transferUtility = new TransferUtility(client);
                var guid = Guid.NewGuid().ToString();
                await transferUtility.UploadAsync(stream, "class-image-frontapp", guid);
                var result = await new PostService().SaveAsync(new PostOwnerEntity 
                {
                    UserName = owner,
                    Url = guid,
                    Coments = new List<Coments>() { new Coments()}
                });

            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            return false;
        }

        public async Task<MemoryStream> GetImageAsync(string url)
        {
            try
            {
                var client = new AmazonS3Client("", "", RegionEndpoint.USEast1);
                var request = new GetObjectRequest
                {
                    BucketName = "class-image-frontapp",
                    Key = url
                };

                using (var response = await client.GetObjectAsync(request))
                using (var stream = response.ResponseStream)
                {
                    MemoryStream memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    return memoryStream;
                }
            }
            catch (Exception e)
            {
                throw;
            }


        }
    }
}
