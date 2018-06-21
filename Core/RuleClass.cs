using System;
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

        public static Rule Get(string UserId, string RuleId)
        {
            var filter = new BsonDocument("id", UserId);
            var userDocument = Collection.RetrieveDocument(filter);

            try
            {
                var rules = JArray.Parse(userDocument["rules"].ToString());
                var rule = rules.SelectToken("$.[?(@.ruleid == '" +RuleId + "')]");
                 Rule ruleDetails = new Rule
                 {
                     Id = (string)rule["ruleid"],
                     Name = (string)rule["name"],
                     Condition = (JObject)rule["condition"],
                     Actions = (JArray)rule["actions"]
                 };
                return ruleDetails;
            }
            catch
            {
                return null;
            }
        }

        public static string Add(string UserId, Rule rule)
        {
            var filter = new BsonDocument("id", UserId);

            var updateDocument = new BsonDocument();

            if(rule.Id == null)
            {
                rule.Id = Guid.NewGuid().ToString();
            }

            var ruleDocument = new BsonDocument{
                {"ruleid", rule.Id},
                {"name", rule.Name},
                {"condition", BsonDocument.Parse(rule.Condition.ToString())},
                {"actions", MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonArray>(rule.Actions.ToString())}
            };

            try
            {
                var userDocument = Collection.RetrieveDocument(filter);
                updateDocument = new BsonDocument {
                    { "$addToSet", new BsonDocument{
                        {"rules", ruleDocument }
                    }}
                };
            }
            catch
            {
                updateDocument = new BsonDocument {
                    { "$addToSet", new BsonArray{
                        new BsonDocument {
                            {"rules", ruleDocument }
                        }
                    }}
                };
            
            }

            Collection.UpdateDocument(filter, updateDocument);

            return rule.Id;
        }

        public static JObject Update(string UserId, Rule updatedRule)
        {
            // everything comment out is due to azure cosmo db issues.
            var currentRule = Get(UserId, updatedRule.Id);

            var filter = new BsonDocument{
                {"id", UserId}/*,
                {"rules.ruleid", currentRule.Id}*/
            };

            var result = new JObject();
            var ruleDocument = new BsonDocument();

            if(currentRule != updatedRule)
            {
                result.Add("changed", new JArray());
                if(currentRule.Name != updatedRule.Name)
                {
                //    ruleDocument.Add("rules.$.name", updatedRule.Name);
                    ((JArray)result.GetValue("changed")).Add("name");
                }
                if(currentRule.Condition.ToString() != updatedRule.Condition.ToString())
                {
                //    ruleDocument.Add("rules.$.condition", BsonDocument.Parse(updatedRule.Condition.ToString()));
                    ((JArray)result.GetValue("changed")).Add("condition");
                }
                if(currentRule.Actions.ToString() != updatedRule.Actions.ToString())
                {
                //    ruleDocument.Add("rules.$.actions", MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonArray>(updatedRule.Actions.ToString()));
                    ((JArray)result.GetValue("changed")).Add("actions");
                }
            }

            //var updateDocument = new BsonDocument {
            //    { "$set", ruleDocument}
            //};
            //Collection.UpdateDocument(filter, updateDocument);

            Delete(UserId, updatedRule.Id);
            Add(UserId, updatedRule);

            return result;
        }

        public static void Delete(string UserId, string RuleId)
        {

            var filter = new BsonDocument {
                {"id", UserId}
            };

            var updateDocument = new BsonDocument{
                {"$pull", new BsonDocument{
                        {"rules", new BsonDocument {
                            {"ruleid", RuleId}
                        }}
                }}
            };

            Collection.UpdateDocument(filter, updateDocument);
        }
    }
}
