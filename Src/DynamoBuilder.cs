namespace Src
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.Model;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class DynamoBuilder
    {
        public const string UserKey = "DynamoConfig:UserTableName";

        public const string UserIdKey = "UserId";

        public const string PayloadAttribute = "Payload";

        public static IWebHostBuilder BuildDynamo(this IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.Configure(app =>
            {
                var env = app.ApplicationServices.GetService<IHostingEnvironment>();
                dynamic startup = Activator.CreateInstance(typeof(Startup), env);

                startup.Configure(app, env);

                var dynamo = app.ApplicationServices.GetService<IAmazonDynamoDB>();
                var logger = app.ApplicationServices.GetService<ILogger<LogContext>>();
                var config = app.ApplicationServices.GetService<IConfiguration>();

                dynamo.BuildUserTable(config, logger).GetAwaiter().GetResult();
            });

            return webHostBuilder;
        }

        private static async Task BuildUserTable(
            this IAmazonDynamoDB client,
            IConfiguration configuration,
            ILogger logger)
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
                            AttributeName = PayloadAttribute,
                            AttributeType = ScalarAttributeType.S
                        }
                    },
                    provisionedThroughput: new ProvisionedThroughput
                    {
                        ReadCapacityUnits = 1,
                        WriteCapacityUnits = 1
                    }
                );

            await client.CreateTableIfNotPresentAsync(tableName, createRequest, logger);
        }

        private static async Task CreateTableIfNotPresentAsync(
            this IAmazonDynamoDB client,
            string tableName,
            CreateTableRequest createRequest,
            ILogger logger)
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
