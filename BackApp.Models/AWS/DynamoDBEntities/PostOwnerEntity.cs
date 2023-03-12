using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackApp.Models.AWS.DynamoDBEntities
{
    public class PostOwnerEntity : DynamoDbEntity
    {
        [DynamoDBHashKey]
        public string UserName { get; set; }
        [DynamoDBRangeKey]
        public string Url { get; set; }
        [DynamoDBProperty]
        public string Title { get; set; }
        [DynamoDBProperty]
        public string Description { get; set; }
        [DynamoDBProperty]
        public int Likes { get; set; }
        [DynamoDBProperty]
        public List<Coments> Coments { get; set; }
    }

    public class Coments
    {
        public string UserName { get; set; }
        public DateTime Date { get; set; }
        public string Post { get; set; }
    }
}
