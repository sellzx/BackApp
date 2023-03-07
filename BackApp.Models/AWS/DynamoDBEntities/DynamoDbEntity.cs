using Amazon.DynamoDBv2.DataModel;
using System;

namespace BackApp.Models.AWS.DynamoDBEntities
{
    public class DynamoDbEntity
    {
        /// <summary>
        /// Fecha de creación
        /// </summary>
        [DynamoDBProperty]
        public DateTime Created { set; get; }

        /// <summary>
        /// Fecha de modificación
        /// </summary>
        [DynamoDBProperty]
        public DateTime Modified { set; get; }

        /// <summary>
        /// Versión
        /// </summary>
        [DynamoDBVersion]
        public int? Version { set; get; }
    }
}
