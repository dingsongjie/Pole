using Pole.ReliableMessage;
using Pole.ReliableMessage.Abstraction;
using Pole.ReliableMessage.Messaging;
using Pole.ReliableMessage.Storage.Mongodb;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pole.ReliableMessage.Storage.Abstraction;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ReliableMessageOptionExtension
    {
        public static ReliableMessageOption AddMongodb(this ReliableMessageOption option, Action<MongodbOption> mongodbOptionConfig)
        {
            option.ReliableMessageOptionExtensions.Add(new MongodbStorageExtension(mongodbOptionConfig));
            return option;
        }
    }
    public class MongodbStorageExtension : IReliableMessageOptionExtension
    {
        private readonly Action<MongodbOption> _mongodbOption;
        public MongodbStorageExtension(Action<MongodbOption> masstransitRabbitmqOption)
        {
            _mongodbOption = masstransitRabbitmqOption;
        }
        public void AddServices(IServiceCollection services)
        {
            services.Configure(_mongodbOption);
            services.AddSingleton<IMessageStorage, MongodbMessageStorage>();
            services.AddSingleton<IMemberShipTable, MongodbMemberShipTable>();

            var mongodbOption = services.BuildServiceProvider().GetRequiredService<IOptions<MongodbOption>>().Value;

            var servers = mongodbOption.Servers.Select(x => new MongoServerAddress(x.Host, x.Port)).ToList();
            var settings = new MongoClientSettings()
            {
                Servers = servers
            };
            var client = new MongoClient(settings);
            var database = client.GetDatabase(mongodbOption.MessageDatabaseName);

            AddMapper();

            InitCollection(mongodbOption, database);

            services.AddSingleton(client);
        }

        private static void InitCollection(MongodbOption mongodbOption, IMongoDatabase database)
        {
            var collectionNames = database.ListCollectionNames().ToList();

            if (!collectionNames.Contains(mongodbOption.ServiceCollectionName))
            {
                database.CreateCollection(mongodbOption.ServiceCollectionName);
                var messageCollection = database.GetCollection<Message>(mongodbOption.ServiceCollectionName);
                AddMessageCollectionIndex(messageCollection);
            }

            if (!collectionNames.Contains(mongodbOption.MembershipCollectionName))
            {
                database.CreateCollection(mongodbOption.MembershipCollectionName);
                var membershipCollection = database.GetCollection<MemberShipTable>(mongodbOption.MembershipCollectionName);
                AddMemberShipTableCollectionIndex(membershipCollection);
            }
        }

        private static void AddMessageCollectionIndex(IMongoCollection<Message> collection)
        {
            List<CreateIndexModel<Message>> createIndexModels = new List<CreateIndexModel<Message>>();

            //var nextRetryUTCTimeIndex = Builders<Message>.IndexKeys.Ascending(m => m.NextRetryUTCTime);
            //CreateIndexModel<Message> nextRetryUTCTimeIndexModel = new CreateIndexModel<Message>(nextRetryUTCTimeIndex, new CreateIndexOptions() { Background = true });
            //createIndexModels.Add(nextRetryUTCTimeIndexModel);

            var AddedUTCTimeUTCTimeIndex = Builders<Message>.IndexKeys.Ascending(m => m.AddedUTCTime);
            CreateIndexModel<Message> AddedUTCTimeIndexModel = new CreateIndexModel<Message>(AddedUTCTimeUTCTimeIndex, new CreateIndexOptions() { Background = true });
            createIndexModels.Add(AddedUTCTimeIndexModel);

            //var messageTypeIdIndex = Builders<Message>.IndexKeys.Ascending(m => m.MessageTypeId);
            //CreateIndexModel<Message> messageTypeIdIndexModel = new CreateIndexModel<Message>(messageTypeIdIndex, new CreateIndexOptions() { Background = true });
            //createIndexModels.Add(messageTypeIdIndexModel);

            collection.Indexes.CreateMany(createIndexModels);
        }
        private static void AddMemberShipTableCollectionIndex(IMongoCollection<MemberShipTable> collection)
        {
            List<CreateIndexModel<MemberShipTable>> createIndexMembershipModels = new List<CreateIndexModel<MemberShipTable>>();

            var serviceNameIndex = Builders<MemberShipTable>.IndexKeys.Ascending(m => m.ServiceName);
            CreateIndexModel<MemberShipTable> serviceNameIndexModel = new CreateIndexModel<MemberShipTable>(serviceNameIndex, new CreateIndexOptions() { Background = true, Unique = true });
            createIndexMembershipModels.Add(serviceNameIndexModel);

            collection.Indexes.CreateMany(createIndexMembershipModels);
        }

        private static void AddMapper()
        {
            BsonClassMap.RegisterClassMap<Message>(cm =>
            {
                cm.AutoMap();
                cm.UnmapMember(m => m.MessageStatus);
                cm.MapIdField(m => m.Id);
                cm.MapMember(m => m.NextRetryUTCTime).SetIsRequired(true);
            });
            BsonClassMap.RegisterClassMap<MemberShipTable>(cm =>
            {
                cm.AutoMap();
                cm.MapIdField(m => m.Id).SetIdGenerator(StringObjectIdGenerator.Instance);
            });
        }
    }
}
