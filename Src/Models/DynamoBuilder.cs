namespace Src
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.Model;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class DynamoBuilder : IDynamoBuilder
    {
        public const string UserKey = "DynamoConfig:UserTableName";

        public const string UserIdKey = "UserId";

        public const string PayloadAttribute = "Payload";

        private readonly IAmazonDynamoDB client;

        private readonly ILogger<LogContext> logger;

        private readonly IConfiguration configuration;

        public DynamoBuilder(
            IAmazonDynamoDB client,
            ILogger<LogContext> logger,
            IConfiguration configuration)
        {
            this.client = client;
            this.logger = logger;
            this.configuration = configuration;
        }

        public async Task BuildUserTable()
        {
            string tableName = configuration[UserKey];

            var createRequest = 
                new CreateTableRequest(
                    tableName: tableName,
                    keySchema: new List<KeySchemaElement>
                    {
                        new KeySchemaElement
                        {
                            AttributeName = UserIdKey,
                            KeyType = KeyType.HASH
                        }
                    },
                    attributeDefinitions: new List<AttributeDefinition>
                    {
                        new AttributeDefinition
                        {
                            AttributeName = UserIdKey,
                            AttributeType = ScalarAttributeType.S
                        }
                    },
                    provisionedThroughput: new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 1,
                        WriteCapacityUnits = 1
                    }
                );

            await this.CreateTableIfNotPresentAsync(tableName, createRequest);
        }

        private async Task CreateTableIfNotPresentAsync(
            string tableName,
            CreateTableRequest createRequest)
        {
            var tableList = await client.ListTablesAsync();

            bool exists = tableList.TableNames.Contains(tableName);
            string message = exists ? $"'{tableName}' table exists" : $"'{tableName}' table does not exist, creating";

            logger.LogInformation(message);

            if (!exists)
            {
                await client.CreateTableAsync(createRequest);
                logger.LogInformation($"Created '{tableName}' table.");
            }
        }

        public class LogContext { }
    }
}
