using BackApp.Models.AWS.DynamoDBEntities;
using BackApp.Services.DynamoDB.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackApp.Services.DynamoDB
{
    public class ChatsService : DynamoDbRepository<ChatsEntity>
    {
        public ChatsService()
        : base(typeof(ChatsEntity), "Chats-table", true)
        {
        }
    }
}
