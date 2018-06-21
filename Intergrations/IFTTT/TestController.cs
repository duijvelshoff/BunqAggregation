using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using bunqAggregation.Core;

namespace bunqAggregation.Intergrations.IFTTT
{
    [Route("ifttt/v1/[controller]")]
    public class TestController : Controller
    {
        [HttpPost]
        [Route("setup")]
        public IActionResult Post()
        {
            bool channel_key = (Config.IFTTT.ChannelKey == Request.Headers["IFTTT-Channel-Key"]);
            bool service_key = (Config.IFTTT.ServiceKey == Request.Headers["IFTTT-Service-Key"]);

            JObject response;

            if (channel_key && service_key)
            {
                var pairs = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("resource", Config.Service.Id ),
                    new KeyValuePair<string, string>("client_id", Config.IFTTT.Test.ClientId ),
                    new KeyValuePair<string, string>("client_secret", Config.IFTTT.Test.Secret),
                    new KeyValuePair<string, string>("username", Config.IFTTT.Test.Username),
                    new KeyValuePair<string, string>("password", Config.IFTTT.Test.Password)
                };
                var content = new FormUrlEncodedContent(pairs);

                var client = new HttpClient();
                var result = client.PostAsync("https://login.microsoftonline.com/duijvelshoff.com/oauth2/token", content).Result;
                var accessToken = JObject.Parse(result.Content.ReadAsStringAsync().Result)["access_token"];
  
                response = new JObject
                {
                    {"data", new JObject{
                        {"accessToken", accessToken},
                        {"samples", new JObject{
                            {"actions",new JObject{
                                {"request_email", new JObject{
                                    {"origin", "597"},
                                    {"recipient", "Arcelia Cooper"},
                                    {"recipient_value", "arcelia.cooper@bunq.nl"},
                                    {"amount", "0.01"},
                                    {"description", "You owe me money!"}
                                }},
                                {"request_iban", new JObject{
                                    {"origin", "597"},
                                    {"recipient", "Sugar Daddy"},
                                    {"recipient_value", "NL65BUNQ9900000188"},
                                    {"amount", "0.01"},
                                    {"description", "You owe me money!"}
                                }},
                                {"transfer_amount", new JObject{
                                    {"origin", "597"},
                                    {"recipient", "Arcelia Cooper"},
                                    {"recipient_iban", "NL54BUNQ9900005139"},
                                    {"amount", "0.01"},
                                    {"description", "Here is some of my money!"}
                                }},
                                {"transfer_percentage", new JObject{
                                    {"origin", "597"},
                                    {"recipient", "Arcelia Cooper"},
                                    {"recipient_iban", "NL54BUNQ9900005139"},
                                    {"percentage", "50"},
                                    {"description", "Here is half of my money!"}
                                }},
                                {"transfer_full_saldo", new JObject{
                                    {"origin", "597"},
                                    {"recipient", "Arcelia Cooper"},
                                    {"recipient_iban", "NL54BUNQ9900005139"},
                                    {"description", "Here is all of my money!"}
                                }},
                                {"transfer_amount_other_account", new JObject{
                                    {"origin", "597"},
                                    {"recipient_iban", "NL81BUNQ9900006496"},
                                    {"amount", "0.01"},
                                    {"description", "Here is some of my money!"}
                                }},
                                {"transfer_percentage_other_account", new JObject{
                                    {"origin", "597"},
                                    {"recipient_iban", "NL81BUNQ9900006496"},
                                    {"percentage", "50"},
                                    {"description", "Here is half of my money!"}
                                }},
                                {"transfer_full_saldo_other_account", new JObject{
                                    {"origin", "597"},
                                    {"recipient_iban", "NL81BUNQ9900006496"},
                                    {"description", "Here is all of my money!"}
                                }}
                            }},
                            {"triggers", new JObject {
                                {"mutation", new JObject {
                                    {"account", "597"}
                                }}
                            }}
                        }}
                    }}
                };
                return StatusCode(200, response);
            }
            else
            {
                return StatusCode(401, ErrorMessages.NoServiceKeyOrChannelKey());
            }
        }
    }
}
