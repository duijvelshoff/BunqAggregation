using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;
using bunqAggregation.Core;

namespace bunqAggregation.Intergrations.bunq
{
    [Route("api/[controller]")]
    public class CallBackController : Controller
    {
        public void Post([FromBody] JObject Content)
        {
            List<Rule> rules = new List<Rule>();

            var paymentMutation = Content["NotificationUrl"]["object"]["Payment"];

            int recipient = 0;
            string recipient_iban = "";
            string origin_name = "";
            string origin_iban = "";
            string description = "";
            string amount = "";

            if(paymentMutation != null)
            {
                recipient = (int)paymentMutation["monetary_account_id"];
                recipient_iban = (string)paymentMutation["alias"]["iban"];
                origin_name = (string)paymentMutation["counterparty_alias"]["display_name"];
                origin_iban = (string)paymentMutation["counterparty_alias"]["iban"];
                description = (string)paymentMutation["description"];
                amount = (string)paymentMutation["amount"]["value"];   
            }

            Account account = Account.Get(null,recipient);

            var filter = new BsonDocument {
                {"$and", new BsonArray {
                    new BsonDocument {
                        { "bunq", new BsonDocument {
                            {"userid", account.UserId}
                    }}},
                    new BsonDocument {
                        {"rules", new BsonDocument {
                            {"$exists", true}
                    }}}
                }}
            };

            var allDocuments = Collection.RetrieveDocuments(filter);
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
                    if ((string)rule.Condition["type"] == "mutation")
                    {
                        Regex descRegex = new Regex((string)rule.Condition["description"]);

                        if (
                            origin_iban == (string)rule.Condition["origin"]["iban"] &
                            recipient_iban == (string)rule.Condition["destination"]["iban"] &
                            descRegex.IsMatch(description)
                        )
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
                                    JObject metadata = new JObject {
                                        {"callback", new JObject{
                                                {"amount", amount}
                                            }}
                                    };
                                    action.Add("metadata", metadata);
                                    Payment.Execute((string)userDocument["id"], action);
                                }
                                if (action.ContainsKey("request"))
                                {
                                    JObject metadata = new JObject {
                                        {"callback", new JObject{
                                                {"amount", amount}
                                            }}
                                    };
                                    action.Add("metadata", metadata);
                                    PaymentRequest.Execute((string)userDocument["id"], action);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
