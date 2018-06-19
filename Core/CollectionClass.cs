using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core;

namespace bunqAggregation.Core
{
    public class Collection
    {
        public static void CreateDocument(BsonDocument document)
        {
            var conn = new MongoClient("mongodb://" + Config.MongoDataBase.Url + "/?ssl=true&replicaSet=globaldb");
            var database = conn.GetDatabase(Config.MongoDataBase.Database);
            var collection = database.GetCollection<BsonDocument>(Config.MongoDataBase.Collection);
            collection.InsertOne(document);
        }

        public static void UpdateDocument(BsonDocument filter, BsonDocument document)
        {
            var conn = new MongoClient("mongodb://" + Config.MongoDataBase.Url + "/?ssl=true&replicaSet=globaldb");
            var database = conn.GetDatabase(Config.MongoDataBase.Database);
            var collection = database.GetCollection<BsonDocument>(Config.MongoDataBase.Collection);
            collection.UpdateOne(filter, document);
        }

        public static BsonDocument RetrieveDocument(BsonDocument filter)
        {
            var conn = new MongoClient("mongodb://" + Config.MongoDataBase.Url + "/?ssl=true&replicaSet=globaldb");
            var database = conn.GetDatabase(Config.MongoDataBase.Database);
            var collection = database.GetCollection<BsonDocument>(Config.MongoDataBase.Collection);

            try
            {
                return collection.Find(filter).First();
            }
            catch
            {
                return null;
            }
        }

        public static List<BsonDocument> RetrieveDocuments(BsonDocument filter)
        {
            var conn = new MongoClient("mongodb://" + Config.MongoDataBase.Url + "/?ssl=true&replicaSet=globaldb");
            var database = conn.GetDatabase(Config.MongoDataBase.Database);
            var collection = database.GetCollection<BsonDocument>(Config.MongoDataBase.Collection);

            try
            {
                return collection.Find(filter).ToList();
            }
            catch
            {
                return null;
            }
        }

        public static bool Registerd(string UserId)
        {
            bool Value = false;
            var Filter = new BsonDocument("id", UserId);
            try
            {
                var UserDocument = RetrieveDocument(Filter)["id"];
                Value = true;
            }
            catch
            {
                Value = false;
            }
            return Value;
        }
    }
}
