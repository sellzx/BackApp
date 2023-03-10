using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using BackApp.Models.Input;
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
            var client = new AmazonS3Client("AKIAVXXTXX443DA5PE7T", "wwn5j6JrXgTPa77eknhbfS29leBf/x4GhJ8EX6DH", RegionEndpoint.USEast1);


            try
            {
                var stream = new MemoryStream(imageBytes);
                stream.Seek(0, SeekOrigin.Begin);
                var transferUtility = new TransferUtility(client);
                await transferUtility.UploadAsync(stream, "class-image-frontapp", Guid.NewGuid().ToString());

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
    }
}
