using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackApp.Models.AWS.DynamoDBEntities
{
    public class LoginServiceEntity : DynamoDbEntity
    {
        [DynamoDBHashKey]
        public string EmailAdress { get; set; }
        [DynamoDBProperty]
        public string Password { get; set; }
        [DynamoDBProperty]
        public string Name { get; set; }
        [DynamoDBProperty]
        public string LastName { get; set; }
    }
}
