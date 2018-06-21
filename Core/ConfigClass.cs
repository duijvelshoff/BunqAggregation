using System;

namespace bunqAggregation.Core
{
    public static class Config
    {
        public static class Service
        {
            public static string Url = Environment.GetEnvironmentVariable("SERVICE_URL");
            public static string Id = Environment.GetEnvironmentVariable("SERVICE_ID");
        }

        public static class MongoDataBase
        {
            public static string Url = Environment.GetEnvironmentVariable("MONGODB_URL");
            public static string Database = Environment.GetEnvironmentVariable("MONGODB_DATABASE");
            public static string Collection = Environment.GetEnvironmentVariable("MONGODB_COLLECTION");
        }

        public static string bunqApiKey = Environment.GetEnvironmentVariable("BUNQ_API_KEY");

        public static class IFTTT
        {
            public static string ChannelKey = Environment.GetEnvironmentVariable("IFTTT_CHANNEL_KEY");
            public static string ServiceKey = Environment.GetEnvironmentVariable("IFTTT_SERVICE_KEY");

            public static class Test
            {
                public static string Username = Environment.GetEnvironmentVariable("IFTTT_TEST_USERNAME");
                public static string Password = Environment.GetEnvironmentVariable("IFTTT_TEST_PASSWORD");
                public static string ClientId = Environment.GetEnvironmentVariable("IFTTT_CLIENT_ID");
                public static string Secret = Environment.GetEnvironmentVariable("IFTTT_CLIENT_SECRET");
            } 
        }
    }
}
