using System.Collections.Generic;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;

namespace bunqAggregation.Core
{
    public class Rule
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public JObject Condition { get; set; }
        public JArray Actions { get; set; }

        public static List<Rule> List(string UserId)
        {
            List<Rule> result = new List<Rule>();

            var filter = new BsonDocument("id", UserId);
            var userDocument = Collection.RetrieveDocument(filter);
            try
            {
                foreach (var rule in JArray.Parse(userDocument["rules"].ToString()))
                {
                    Rule ruleDetails = new Rule {
                        Id = (string)rule["ruleid"],
                        Name = (string)rule["name"],
                        Condition = (JObject)rule["condition"],
                        Actions = (JArray)rule["actions"]
                    };
                    result.Add(ruleDetails);
                }
                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}
