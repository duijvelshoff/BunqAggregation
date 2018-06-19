using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using bunqAggregation.Core;
using bunqAggregation.Intergration.bunq;

namespace bunqAggregation.Intergration.IFTTT
{
    [Route("/ifttt/v1/[controller]")]
    public class TriggersController : Controller
    {
        
        [Authorize(ActiveAuthenticationSchemes = Config.Client.Service)]
        [HttpPost]
        [Route("mutation")]
        public IActionResult Mutation([FromBody]JObject content)
        {
            string UserId = null;

            foreach (var claim in User.Claims)
            {
                if (claim.Type == "sub")
                {
                    UserId = claim.Value;
                }
            }

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


        [Authorize(ActiveAuthenticationSchemes = Config.Client.Service)]
        [HttpPost]
        [Route("mutation/fields/account/options")]
        public IActionResult ListAccountsRO()
        {
            var response = new JObject();
            string UserId = null;

            foreach (var claim in User.Claims)
            {
                if (claim.Type == "sub")
                {
                    UserId = claim.Value;
                }
            }

            if (Collection.Registerd(UserId))
            {
                List<Account> Accounts = Account.List(UserId);
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
