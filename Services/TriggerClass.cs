using System;
using System.Collections.Generic;
using System.Threading;
using MongoDB.Bson;
using NCrontab;
using Newtonsoft.Json.Linq;
using bunqAggregation.Core;
using bunqAggregation.Intergrations.bunq;

namespace bunqAggregation.Services
{
    public class Trigger
    {
        public static void Job()
        {
            while (true)
            {
                var start = DateTime.Now;
                List<Rule> rules = new List<Rule>();

                var Filter = new BsonDocument {
                    {"rules", new BsonDocument {
                        {"$exists", true}
                    }}
                };

                var allDocuments = Collection.RetrieveDocuments(Filter);

                foreach (var userDocument in allDocuments)
                {
                    foreach (var rule in JArray.Parse(userDocument["rules"].ToString()))
                    {
                        Rule ruleDetails = new Rule
                        {
                            Id = (string)rule["ruleid"],
                            Name = (string)rule["name"],
                            Condition = (JObject)rule["condition"],
                            Actions = (JArray)rule["actions"]
                        };
                        rules.Add(ruleDetails);
                    }

                    foreach (var rule in rules)
                    {
                        if ((string)rule.Condition["type"] == "trigger")
                        {
                            if ( (CrontabSchedule.Parse((string)rule.Condition["when"])).GetNextOccurrence(DateTime.Now.AddMinutes(-1)).ToString("yyyy-MM-dd HH:mm") == start.ToString("yyyy-MM-dd HH:mm"))
                            {
                                foreach (JObject action in rule.Actions)
                                {
                                    if (action.ContainsKey("email"))
                                    {
                                        Console.WriteLine(action);
                                        //TODO: Create Mail on trigger!
                                    }
                                    if (action.ContainsKey("payment"))
                                    {
                                        Console.WriteLine(action);
                                        Payment.Execute((string)userDocument["id"], action);
                                    }
                                    if (action.ContainsKey("request"))
                                    {
                                        Console.WriteLine(action);
                                        PaymentRequest.Execute((string)userDocument["id"], action);
                                    }
                                }
                            }
                        }
                    }
                }
                var end = DateTime.Now;
                var timeleft = start.AddMinutes(1) - end;

                Filter = new BsonDocument {
                    {"trigger", "job"}
                };
                var Result = Collection.RetrieveDocument(Filter);

                if (Result != null)
                {
                    var Document = new BsonDocument {
                        { "$set", new BsonDocument {
                            {"lastrun", end}
                        }}
                    };
                    Collection.UpdateDocument(Filter, Document);
                }
                else
                {
                    var Document = new BsonDocument {
                        {"trigger", "job"},
                        {"lastrun", end}
                    };
                    Collection.CreateDocument(Document);
                }

                Thread.Sleep((int)timeleft.TotalMilliseconds);
            }
        }
    }
}