

namespace Src.Models.User
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.Model;

    using Microsoft.Extensions.Configuration;

    using Newtonsoft.Json;

    public class NovelGramUserClient : INovelGramUserClient
    {
        private readonly string userTableName;

        private readonly ICurrentUserManager currentUserManager;

        private readonly IAmazonDynamoDB dynamoClient;

        public NovelGramUserClient(
            IAmazonDynamoDB dynamoClient,
            ICurrentUserManager currentUserManager,
            IConfiguration configuration)
        {
            this.userTableName = configuration[DynamoBuilder.UserKey];
            this.currentUserManager = currentUserManager ?? throw new ArgumentNullException(nameof(currentUserManager));
            this.dynamoClient = dynamoClient ?? throw new ArgumentNullException(nameof(dynamoClient));
        }

        public async Task<IList<NovelGramUser>> GetUserBatchAsync(IList<string> userIds)
        {
            if (userIds == null || !userIds.Any())
            {
                return null;
            }

            var batchRequest = new BatchGetItemRequest
            {
                RequestItems = new Dictionary<string, KeysAndAttributes>
                {
                    {
                        this.userTableName, new KeysAndAttributes
                        {
                            Keys = new List<Dictionary<string, AttributeValue>>(userIds.Select(GetKey))
                        }
                    }
                }
            };

            var response = await this.dynamoClient.BatchGetItemAsync(batchRequest);
            return response?.Responses?.FirstOrDefault().Value?.Select(UserFromItem).ToArray();
        }

        public async Task<NovelGramUser> GetUserAsync(string userId)
        {
            var request = new GetItemRequest
            {
                TableName = this.userTableName,
                Key = GetKey(userId)
            };

            var userItem = await this.dynamoClient.GetItemAsync(request);
            return userItem != null && userItem.IsItemSet ? UserFromItem(userItem.Item) : null;
        }

        public async Task SaveUserAsync(NovelGramUser user)
        {
            if (user == null)
            {
                return;
            }

            string payload = JsonConvert.SerializeObject(user);

            var putRequest = new PutItemRequest
            {
                TableName = this.userTableName,
                
                Item = new Dictionary<string, AttributeValue>
                {
                    { DynamoBuilder.UserKey, new AttributeValue(user.UserId ) },
                    { DynamoBuilder.PayloadAttribute, new AttributeValue(payload) }
                }
            };

            var response = await this.dynamoClient.PutItemAsync(putRequest);

            if (response.HttpStatusCode != HttpStatusCode.OK)
            {
                throw new InvalidOperationException($"User save status did not indicate success: {response.HttpStatusCode}");
            }
        }

        public async Task<NovelGramUser> GetCurrentUserAsync()
        {
            string userId = await this.currentUserManager.GetCurrentUserIdAsync();
            return await this.GetUserAsync(userId);
        }

        private static Dictionary<string, AttributeValue> GetKey(string userId)
        {
            return new Dictionary<string, AttributeValue>
            {
                { DynamoBuilder.UserKey, new AttributeValue(userId) }
            };
        }

        private static NovelGramUser UserFromItem(Dictionary<string, AttributeValue> item)
        {
            string payload = item[DynamoBuilder.PayloadAttribute].S;
            return !string.IsNullOrWhiteSpace(payload) ? JsonConvert.DeserializeObject<NovelGramUser>(payload) : null;
        }
    }
}
