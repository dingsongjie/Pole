using Pole.ReliableMessage.Abstraction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pole.ReliableMessage.Storage.Abstraction;

namespace Pole.ReliableMessage.Storage.Mongodb
{
    class MongodbMemberShipTableManager : IMemberShipTableManager
    {
        private readonly MongoClient _mongoClient;
        private readonly MongodbOption _mongodbOption;
        private readonly ILogger _logger;
        public MongodbMemberShipTableManager(IConfiguration configuration, MongoClient mongoClient, IOptions<MongodbOption> mongodbOption, ILogger<MongodbMemberShipTableManager> logger)
        {
            _mongoClient = mongoClient;
            _mongodbOption = mongodbOption.Value;
            _logger = logger;
        }
        private IMongoDatabase GetActiveMessageDatabase(string activeMessageDatabase)
        {
            return _mongoClient.GetDatabase(activeMessageDatabase);
        }
        private IMongoCollection<MemberShipTable> GetCollection()
        {
            var database = GetActiveMessageDatabase(_mongodbOption.MessageDatabaseName);
            var messageCollectionName = _mongodbOption.MembershipCollectionName;
            var collection = database.GetCollection<MemberShipTable>(messageCollectionName);
            return collection;
        }
        public async Task<bool> AddCheckerServiceInstanceAndDeleteOthers(string ipAddress, DateTime aliveUTCTime)
        {
            var collection = GetCollection();
            var deleteResult = await collection.DeleteManyAsync(m => m.ServiceName == _mongodbOption.ServiceCollectionName);
            MemberShipTable memberShipTable = new MemberShipTable(_mongodbOption.ServiceCollectionName, ipAddress, aliveUTCTime);
            await collection.InsertOneAsync(memberShipTable);
            return true;
        }

        public async Task<string> GetPendingMessageCheckerServiceInstanceIp(DateTime iamAliveEndTime)
        {
            var collection = GetCollection();

            var instances = (await collection.FindAsync(m => m.ServiceName == _mongodbOption.ServiceCollectionName && m.IAmAliveUTCTime >= iamAliveEndTime)).ToList();
            if (instances.Count > 1)
            {
                _logger.LogInformation($"Current time have {instances.Count} PendingMessageChecker in {_mongodbOption.ServiceCollectionName} service , I will delete  the extra instances");
                var currentInstance = instances.FirstOrDefault();
                var extraInstances = instances.Remove(currentInstance);
                instances.ForEach(async n =>
                {
                    await collection.DeleteOneAsync(m => m.Id == n.Id);
                });
                _logger.LogInformation($"Extra PendingMessageChecker instances in {_mongodbOption.ServiceCollectionName} service deleted successfully");
                return currentInstance.PendingMessageCheckerIp;
            }
            else if (instances.Count == 1)
            {
                return instances.FirstOrDefault().PendingMessageCheckerIp;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> IsPendingMessageCheckerServiceInstance(string ipAddress)
        {
            var collection = GetCollection();

            var instances = (await collection.FindAsync(m => m.ServiceName == _mongodbOption.ServiceCollectionName && m.PendingMessageCheckerIp== ipAddress)).FirstOrDefault();
            if (instances != null)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateIAmAlive(string ipAddress,DateTime dateTime)
        {
            var collection = GetCollection();
            var filter = Builders<MemberShipTable>.Filter.Where(m => m.ServiceName == _mongodbOption.ServiceCollectionName && m.PendingMessageCheckerIp == ipAddress);
            var update = Builders<MemberShipTable>.Update.Set(m=>m.IAmAliveUTCTime,dateTime);
            var result = await collection.UpdateOneAsync(filter, update);
            return true;
        }
    }
}
