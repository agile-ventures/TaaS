using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace AgileVentures.TezPusher.Service
{
    public interface IAzureStorageHelper
    {
        Task<List<T>> ReadAllEntities<T>(string tableName, string partitionKey = null, int count = 0) where T : TableEntity, new();

        Task<List<T>> ReadAllEntitiesByRowKey<T>(string tableName, string rowKey = null, int count = 0) where T : TableEntity, new();
    }

    public class AzureStorageHelper : IAzureStorageHelper
    {
        private Dictionary<string, CloudTable> TableReferences => new Dictionary<string, CloudTable>();
        private readonly string _connectionString;

        public AzureStorageHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        private async Task<CloudTable> GetTableReference(string tableName)
        {
            if (TableReferences.ContainsKey(tableName))
            {
                return TableReferences[tableName];
            }

            var storageAccount = CloudStorageAccount.Parse(_connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);

            var tableExists = await table.ExistsAsync();
            if (!tableExists)
            {
                // Create the table if it doesn't exist.
                //This is always returning 409 and it spams the app insights logs => we're only calling it when table doesn't exists
                await table.CreateAsync();
            }

            TableReferences.Add(tableName, table);
            return table;
        }

        public async Task<List<T>> ReadAllEntities<T>(string tableName, string partitionKey = null, int count = 0) where T : TableEntity, new()
        {
            var table = await GetTableReference(tableName);
            return await ReadAllEntitiesByKeys<T>(table, partitionKey: partitionKey, count: count);
        }

        public async Task<List<T>> ReadAllEntitiesByRowKey<T>(string tableName, string rowKey = null, int count = 0) where T : TableEntity, new()
        {
            var table = await GetTableReference(tableName);
            return await ReadAllEntitiesByKeys<T>(table, rowKey: rowKey, count: count);
        }

        private async Task<List<T>> ReadAllEntitiesByKeys<T>(CloudTable table, string partitionKey = null, string rowKey = null, int count = 0) 
            where T : TableEntity, new()
        {
            var entities = new List<T>();
            var tableQuery = new TableQuery<T>();
            if (!string.IsNullOrEmpty(partitionKey) && string.IsNullOrEmpty(rowKey))
            {
                tableQuery = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey));
            }
            else if (string.IsNullOrEmpty(partitionKey) && !string.IsNullOrEmpty(rowKey))
            {
                tableQuery = new TableQuery<T>().Where(TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey));
            }
            else if (!string.IsNullOrEmpty(partitionKey) && !string.IsNullOrEmpty(rowKey))
            {
                throw new ArgumentOutOfRangeException($"Use ReadEntity if you have both keys!");
            }

            if (count > 0)
            {
                tableQuery.TakeCount = count;
            }

            TableContinuationToken token = null;
            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(tableQuery, token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (count > 0 ? token != null && entities.Count <= count : token != null);

            return entities;
        }
    }
}