using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using BackApp.Models.AWS.DynamoDBEntities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackApp.Services.DynamoDB.Repository
{
    public class DynamoDbRepository<T> : IDynamoDbRepository<T> where T : DynamoDbEntity
    {
        /// <summary>
        /// IDynamoDBContext
        /// </summary>
        protected IDynamoDBContext DbContext { get; }

        /// <summary>
        /// DynamoDbRepository
        /// </summary>
        /// <param name="ptype">Type</param>
        /// <param name="ptableName">Name of table</param>
        /// <param name="pconsistentRead"></param>
        /// <param name="pskipVersionCheck"></param>
        public DynamoDbRepository(Type ptype, string ptableName, bool pconsistentRead = false,
            bool pskipVersionCheck = false)
        {
            AWSConfigsDynamoDB.Context.TypeMappings[ptype] = new Amazon.Util.TypeMapping(ptype, ptableName);

            AmazonDynamoDBConfig clientConfig = new AmazonDynamoDBConfig();
            clientConfig.RegionEndpoint = RegionEndpoint.USEast1;
            DbContext = new DynamoDBContext(new AmazonDynamoDBClient("AKIAVXXTXX443DA5PE7T", "wwn5j6JrXgTPa77eknhbfS29leBf/x4GhJ8EX6DH", clientConfig), new DynamoDBContextConfig()
            {
                ConsistentRead = pconsistentRead,
                SkipVersionCheck = pskipVersionCheck
            });
        }

        /// <summary>
        /// Get entity by key
        /// </summary>
        /// <param name="pkey">Key of the Entity</param>
        /// <param name="prangeKey">Param in Entity</param>
        /// <returns>Entity</returns>
        public async Task<T> GetAsync(object pkey, object prangeKey = null)
        {
            try
            {
                return await DbContext.LoadAsync<T>(pkey, prangeKey);
            }
            catch (AmazonDynamoDBException ex)
            {
                Console.WriteLine($"Error in Get{ex}");
            }

            return await Task.FromResult<T>(null);
        }

        /// <summary>
        /// Get all entities
        /// </summary>
        /// <returns>List of the entities</returns>
        public async Task<List<T>> GetAllAsync()
        {
            try
            {
                var options = new DynamoDBOperationConfig();
                var items = new List<T>();

                var scan = DbContext.ScanAsync<T>(new List<ScanCondition>(), options);
                while (!scan.IsDone)
                {
                    var set = await scan.GetNextSetAsync();
                    items.AddRange(set);
                }

                return items;
            }
            catch (AmazonDynamoDBException ex)
            {
                Console.WriteLine($"Error in GetAll{ex}");
            }

            return await Task.FromResult<List<T>>(null);
        }

        /// <summary>
        /// Load Entity
        /// </summary>
        /// <param name="pkeys">Key of the Entity</param>
        /// <returns>List of the entity</returns>
        public async Task<IEnumerable<T>> LoadAsync(IEnumerable<Tuple<object, object>> pkeys)
        {
            try
            {
                var options = new DynamoDBOperationConfig();
                var documents = new List<T>();
                var batch = DbContext.CreateBatchGet<T>(options);

                foreach (var tuple in pkeys)
                {
                    batch.AddKey(tuple.Item1, tuple.Item2);
                }

                await batch.ExecuteAsync();
                documents.AddRange(batch.Results);
                return documents;
            }
            catch (AmazonDynamoDBException ex)
            {
                Console.WriteLine($"Error in Load{ex}");
            }

            return await Task.FromResult<IEnumerable<T>>(null);
        }

        /// <summary>
        /// Query for search entity
        /// </summary>
        /// <param name="pkey">Key of the Entity</param>
        /// <param name="pindexName">Param in Entity</param>
        /// <returns>List of the entity</returns>
        public async Task<IEnumerable<T>> QueryAsync(object pkey, string pindexName = null)
        {
            try
            {
                var options = new DynamoDBOperationConfig();
                if (null != pindexName)
                {
                    options.IndexName = pindexName;
                }

                var documents = new List<T>();
                var query = DbContext.QueryAsync<T>(pkey, options);

                while (!query.IsDone)
                {
                    var set = await query.GetNextSetAsync();
                    documents.AddRange(set);
                }

                return documents;
            }
            catch (AmazonDynamoDBException ex)
            {
                Console.WriteLine($"Error in Query{ex}");
            }

            return await Task.FromResult<IEnumerable<T>>(null);
        }

        /// <summary>
        /// Save entity
        /// </summary>
        /// <param name="pentity">Entity</param>
        /// <returns>Task os type bool</returns>
        public async Task<bool> SaveAsync(T pentity)
        {
            try
            {
                if (pentity.Created == DateTime.MinValue)
                {
                    pentity.Created = DateTime.UtcNow;
                }

                pentity.Modified = DateTime.UtcNow;
                await DbContext.SaveAsync(pentity);

                return true;
            }
            catch (AmazonDynamoDBException ex)
            {
                Console.WriteLine($"Excepción: {ex.Message}, StackTrace: {ex.StackTrace}, InnerException: {ex.InnerException}");

                if (ex is Amazon.DynamoDBv2.Model.ConditionalCheckFailedException)
                {
                    return false;
                }

                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<bool> SaveBatchAsync(IEnumerable<T> entities)
        {
            try
            {
                var options = new DynamoDBOperationConfig
                {
                    SkipVersionCheck = true
                };

                var batchWork = DbContext.CreateBatchWrite<T>(options);
                batchWork.AddPutItems(entities);
                await batchWork.ExecuteAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción: {ex.Message}, StackTrace: {ex.StackTrace}, InnerException: {ex.InnerException}");
                return false;
            }
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            try
            {
                await DbContext.DeleteAsync(entity);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción: {ex.Message}, StackTrace: {ex.StackTrace}, InnerException: {ex.InnerException}");
                return false;
            }
        }

        public async Task<bool> DeleteBatchAsync(IEnumerable<T> entities)
        {
            try
            {
                var options = new DynamoDBOperationConfig
                {
                    SkipVersionCheck = true
                };

                var batchWork = DbContext.CreateBatchWrite<T>(options);
                batchWork.AddDeleteItems(entities);
                await batchWork.ExecuteAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción: {ex.Message}, StackTrace: {ex.StackTrace}, InnerException: {ex.InnerException}");
                return false;
            }
        }

        /// <summary>
        /// Delete all in Table
        /// </summary>
        /// <returns></returns>
        public async Task<bool> DeleteAllInTable()
        {
            try
            {
                var options = new DynamoDBOperationConfig
                {
                    SkipVersionCheck = true
                };

                var scan = await DbContext.ScanAsync<T>(new List<ScanCondition>(), options).GetRemainingAsync();
                var batchWork = DbContext.CreateBatchWrite<T>(options);
                batchWork.AddDeleteItems(scan);
                await batchWork.ExecuteAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción: {ex.Message}, StackTrace: {ex.StackTrace}, InnerException: {ex.InnerException}");
                return false;
            }
        }

        public async Task<IEnumerable<T>> ScanAsync(List<ScanCondition> scanConditions = null)
        {
            var options = new DynamoDBOperationConfig
            {
                SkipVersionCheck = true
            };

            var scan = await DbContext.ScanAsync<T>(scanConditions != null ? scanConditions : new List<ScanCondition>(), options).GetRemainingAsync();
            return scan;
        }
    }

}