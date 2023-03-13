using BackApp.Models.AWS.DynamoDBEntities;
using BackApp.Services.DynamoDB.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackApp.Services.DynamoDB
{
    public class PostService : DynamoDbRepository<PostOwnerEntity>
    {
        public PostService()
        : base(typeof(PostOwnerEntity), "Posts-table", true)
        {
        }
    }
}
