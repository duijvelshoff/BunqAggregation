using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using bunqAggregation.Core;
using bunqAggregation.Intergrations.bunq;

namespace bunqAggregation.Intergrations.IFTTT
{
    [Route("/ifttt/v1/[controller]")]
    public class TriggersController : Controller
    {
        
        [Authorize]
        [HttpPost]
        [Route("mutation")]
        public IActionResult Mutation([FromBody]JObject content)
        {
            string userObjectID = (User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value;

            JObject response;

            var triggerIdentity = content["trigger_identity"];

            if (triggerIdentity == null)
            {
                return StatusCode(400, ErrorMessages.TriggerIdentityIsMissing());
            }


            var triggerFields = content["triggerFields"];

            if (triggerFields == null)
            {
                return StatusCode(400, ErrorMessages.TriggerFieldsAreMissing());
            }

            var account = triggerFields["account"];

            if (
                account == null 
            )
            {
                return StatusCode(400, ErrorMessages.MandatoryFieldAreMissing());
            }

            if (content["limit"] != null)
            {
                if ((int)content["limit"] == 1) 
                {
                    response = new JObject {
                        {"data", new JArray {
                                new JObject {
                                    {"mutation", "money in the bank!"},
                                    {"created_at", DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ssZ")},
                                    {"meta", new JObject {
                                        {"id", Guid.NewGuid().ToString() },
                                        {"timestamp", (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds}
                                    }}
                                }
                            }
                        }
                    };
                }
                else
                {
                    response = new JObject {
                        {"data", new JArray()}
                    };
                }
            }
            else
            {
                response = new JObject {
                    {"data", new JArray {
                            new JObject {
                                {"mutation", "money in the bank!"},
                                {"created_at", DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ssZ")},
                                {"meta", new JObject {
                                    {"id", Guid.NewGuid().ToString() },
                                    {"timestamp", (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds}
                                }}
                            },
                            new JObject {
                                {"mutation", "money in the bank!"},
                                {"created_at", DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ssZ")},
                                {"meta", new JObject {
                                    {"id", Guid.NewGuid().ToString() },
                                    {"timestamp", (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds}
                                }}
                            },
                            new JObject {
                                {"mutation", "money in the bank!"},
                                {"created_at", DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ssZ")},
                                {"meta", new JObject {
                                    {"id", Guid.NewGuid().ToString() },
                                    {"timestamp", (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds}
                                }}
                            }
                        }
                    }
                };
            }
            return StatusCode(200,response);
        }


        [Authorize]
        [HttpPost]
        [Route("mutation/fields/account/options")]
        public IActionResult ListAccountsRO()
        {
            string userObjectID = (User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value;

            var response = new JObject();
         
            if (Collection.Registerd(userObjectID))
            {
                List<Account> Accounts = Account.List(userObjectID);
                if (Accounts.Count > 0)
                {
                    response.Add("data", new JArray());
                    foreach (Account account in Accounts)
                    {
                        ((JArray)response.GetValue("data")).Add(new JObject(
                            new JProperty("label", account.Description),
                            new JProperty("value", account.IBAN)
                        ));
                    }
                }
                else
                {
                    response.Add("errors", new JArray {
                        new JObject {
                            {"message", "The user has no bunq Current Accounts active."}
                        }
                    });
                }
            }
            else
            {
                response.Add("errors", new JArray {
                    new JObject {
                        {"message", "The user has no bunq Current Account added yet!"}
                    }
                });
            }
            return StatusCode(200, response);
        }
    }
}
