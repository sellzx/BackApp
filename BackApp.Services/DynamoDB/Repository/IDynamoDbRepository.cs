using Amazon.DynamoDBv2.DataModel;
using BackApp.Models.AWS.DynamoDBEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackApp.Services.DynamoDB.Repository
{
    public interface IDynamoDbRepository<T> where T : DynamoDbEntity
    {
        Task<bool> SaveAsync(T pentity);

        Task<bool> SaveBatchAsync(IEnumerable<T> entities);

        Task<T> GetAsync(object pkey, object prangeKey = null);

        Task<List<T>> GetAllAsync();

        Task<IEnumerable<T>> QueryAsync(object key, string indexName = null);

        Task<IEnumerable<T>> LoadAsync(IEnumerable<Tuple<object, object>> keys);

        Task<IEnumerable<T>> ScanAsync(List<ScanCondition> scanConditions = null);

        Task<bool> DeleteAsync(T entity);

        Task<bool> DeleteBatchAsync(IEnumerable<T> entities);

        Task<bool> DeleteAllInTable();
    }
}
