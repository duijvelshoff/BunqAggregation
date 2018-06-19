using System;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using bunqAggregation.Core;

namespace bunqAggregation.Intergration.IFTTT
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
                var disco = DiscoveryClient.GetAsync(Config.Authority);
                if (disco.Result.IsError)
                {
                    Console.WriteLine(disco.Result.IsError);
                    return StatusCode(500);
                }

                var tokenClient = new TokenClient(disco.Result.TokenEndpoint, "bunqaggregation_backend", Config.Service.Secret);
                var tokenResponse = tokenClient.RequestResourceOwnerPasswordAsync(Config.IFTTT.Test.Username, Config.IFTTT.Test.Password, "ifttt openid profile");

                if (tokenResponse.Result.IsError)
                {
                    Console.WriteLine(tokenResponse.Result.IsError);
                    return StatusCode(500);
                }

                response = new JObject
                {
                    {"data", new JObject{
                        {"accessToken", tokenResponse.Result.AccessToken},
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
