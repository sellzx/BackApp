using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BackApp.Models.AWS.DynamoDBEntities
{
    public class ChatsEntity : DynamoDbEntity
    {
        [DynamoDBHashKey]
        public string Owner { get; set; }
        [DynamoDBProperty]
        public string ConversationId { get; set; }
        [DynamoDBProperty]
        public List<string> Participants { get; set; }
        [DynamoDBProperty]
        public List<Conversation> Conversation { get; set; }
    }

    public class Conversation
    {
        public DateTime Date { get; set; }
        public string CreatedBy { get; set; }
        public string Message { get; set; }
    }
}
