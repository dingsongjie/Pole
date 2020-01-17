using Pole.ReliableMessage.Abstraction;
using Pole.ReliableMessage.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Pole.ReliableMessage.Storage.Abstraction;
using Pole.Domain;

namespace Pole.ReliableMessage.Storage.Mongodb
{
    class MongodbMessageStorage : IMessageStorage
    {
        private readonly MongoClient _mongoClient;
        private readonly MongodbOption _mongodbOption;
        private readonly ILogger<MongodbMessageStorage> _logger;
        public MongodbMessageStorage(MongoClient mongoClient, IOptions<MongodbOption> mongodbOption, ILogger<MongodbMessageStorage> logger)
        {
            _mongoClient = mongoClient;
            _mongodbOption = mongodbOption.Value;
            _logger = logger;
        }
        private IMongoDatabase GetActiveMessageDatabase(string messageDatabase)
        {
            return _mongoClient.GetDatabase(messageDatabase);
        }
        private IMongoCollection<Message> GetCollection()
        {
            var database = GetActiveMessageDatabase(_mongodbOption.MessageDatabaseName);
            var messageCollectionName = _mongodbOption.ServiceCollectionName;
            var collection = database.GetCollection<Message>(messageCollectionName);
            return collection;
        }
        public async Task<bool> Add(Message message)
        {
            IMongoCollection<Message> collection = GetCollection();

            await collection.InsertOneAsync(message);
            return true;
        }

        public async Task<bool> CheckAndUpdateStatus(Expression<Func<Message, bool>> filter, MessageStatus messageStatus)
        {
            IMongoCollection<Message> collection = GetCollection();

            var update = Builders<Message>.Update.Set(m => m.MessageStatusId, messageStatus.Id);
            var beforeDoc = await collection.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<Message, Message>() { ReturnDocument = ReturnDocument.Before });
            if (beforeDoc == null)
            {
                throw new Exception("IMessageStorage.CheckAndUpdateStatus Error ,Message not found in Storage");
            }
            if (beforeDoc.MessageStatusId == messageStatus.Id)
            {
                return false;
            }
            return true;
        }

        public async Task<List<Message>> GetMany(Expression<Func<Message, bool>> filter, int count)
        {
            IMongoCollection<Message> collection = GetCollection();

            var list = await collection.Find(filter).Limit(count).ToListAsync();
            list.ForEach(m =>
            {
                m.MessageStatus = Enumeration.FromValue<MessageStatus>(m.MessageStatusId);
            });
            return list;
        }

        public async Task<bool> Save(IEnumerable<Message> messages)
        {
            var count = messages.Count();
            _logger.LogDebug($"MongodbMessageStorage Save begin, Messages count: {messages.Count()}");
            if (count == 0)
            {
                _logger.LogDebug($"MongodbMessageStorage Save successfully, saved count: 0");
                return true;
            }
            IMongoCollection<Message> collection = GetCollection();

            var models = new List<WriteModel<Message>>();
            foreach (var message in messages)
            {
                FilterDefinition<Message> filter = Builders<Message>.Filter.Where(m => m.Id == message.Id);
                UpdateDefinition<Message> update = Builders<Message>.Update
                    .Set(m => m.MessageStatusId, message.MessageStatus.Id)
                    .Set(m => m.RetryTimes, message.RetryTimes)
                    .Set(m => m.NextRetryUTCTime, message.NextRetryUTCTime);

                var model = new UpdateOneModel<Message>(filter, update);
                models.Add(model);
            }
            var result = await collection.BulkWriteAsync(models, new BulkWriteOptions { IsOrdered = false });

            _logger.LogDebug($"MongodbMessageStorage Save successfully, saved count: {result.ModifiedCount}");

            return result.IsAcknowledged;
        }

        public async Task<bool> UpdateStatus(Expression<Func<Message, bool>> filter, MessageStatus messageStatus)
        {
            IMongoCollection<Message> collection = GetCollection();

            var update = Builders<Message>.Update.Set(m => m.MessageStatusId, messageStatus.Id);
            var result = await collection.UpdateOneAsync(filter, update);
            return result.IsAcknowledged;
        }

        public async Task<long> Delete(Expression<Func<Message, bool>> filter)
        {
            IMongoCollection<Message> collection = GetCollection();

            var result = await collection.DeleteManyAsync(filter);
            return result.DeletedCount;
        }

        public Task<Message> GetOne(Expression<Func<Message, bool>> filter)
        {
            throw new NotImplementedException();
        }
    }
}
