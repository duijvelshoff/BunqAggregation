using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using bunqAggregation.Core;
using bunqAggregation.Intergrations.bunq;

namespace bunqAggregation.Intergrations.IFTTT
{
    
    [Route("ifttt/v1/[controller]")]
    public class ActionsController : Controller
    {
        [Authorize]
        [HttpPost]
        [Route("request_email")]
        [Route("request_iban")]
        public IActionResult ActionPaymentRequest([FromBody] JObject content)
        {
            string userObjectID = (User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value;

            JObject response;

            var action_details = content["actionFields"];

            if (action_details == null)
            {
                return StatusCode(400, ErrorMessages.ActionFliedsAreMissing());
            }

            var origin = action_details["origin"];
            var recipient = action_details["recipient"];
            var recipient_value = action_details["recipient_value"];
            var amount = action_details["amount"];
            var description = action_details["description"];

            if (
                origin == null ||
                recipient == null ||
                recipient_value == null ||
                amount == null ||
                description == null
            )
            {
                return StatusCode(400, ErrorMessages.MandatoryFieldAreMissing());
            }

            string identifier = Guid.NewGuid().ToString();

            JObject details = new JObject {
                {"request", new JObject {
                    {"origin", new JObject {
                        {"id", origin }
                    }},
                    {"recipient", new JObject {
                        {"name", recipient},
                        {"value", recipient_value}
                    }},
                    {"amount", new JObject{
                        {"value", amount}
                    }},
                    {"description", description}
                }}
            };

            response = new JObject {
                {"data", new JArray {
                        new JObject {
                            {"id", identifier}
                        }
                    }
                }
            };

            if (Request.Headers["IFTTT-Test-Mode"].Equals("1"))
            {
                response.Add("metadata", new JObject {
                    {"requestid", 0}
                });
                return StatusCode(200, response);
            }
            else
            {
                var requestResponse = PaymentRequest.Execute(userObjectID, details);
                response.Add("metadata", new JObject {
                    {"requestid", requestResponse}
                });
                return StatusCode(200, response);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("transfer_amount")]
        [Route("transfer_amount_other_account")]
        public IActionResult ActionPaymentAmount([FromBody] JObject content)
        {
            string userObjectID = (User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value;

            JObject response;

            var action_details = content["actionFields"];

            if (action_details == null)
            {
                return StatusCode(400, ErrorMessages.ActionFliedsAreMissing());
            }

            var origin = action_details["origin"];
            var recipient = action_details["recipient"];
            var recipient_iban = action_details["recipient_iban"];
            var amount = action_details["amount"];
            var description = action_details["description"];

            if (recipient == null)
            {
                recipient = (User.FindFirst("name"))?.Value;
            }

            if (
                origin == null ||
                recipient == null ||
                recipient_iban == null ||
                amount == null ||
                description == null
            )
            {
                return StatusCode(400, ErrorMessages.MandatoryFieldAreMissing());
            }

            string identifier = Guid.NewGuid().ToString();

            JObject details = new JObject {
                {"payment", new JObject {
                    {"origin", new JObject {
                        {"id", origin}
                    }},
                    {"recipient", new JObject {
                        {"name", recipient},
                        {"iban", recipient_iban}
                    }},
                    {"amount", new JObject {
                        {"type", "exact"},
                        {"value", amount}
                    }},
                    {"description", description}
                }}
            };

            response = new JObject {
                {"data", new JArray {
                        new JObject {
                            {"id", identifier}
                        }
                    }
                }
            };

            if (Request.Headers["IFTTT-Test-Mode"].Equals("1"))
            {
                response.Add("metadata", new JObject {
                    {"paymentid", 0}
                });

                return StatusCode(200, response);
            }
            else
            {
                var paymentResponse = Payment.Execute(userObjectID, (JObject)details);

                response.Add("metadata", new JObject {
                    {"paymentid", paymentResponse}
                });

                return StatusCode(200, response);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("transfer_percentage")]
        [Route("transfer_percentage_other_account")]
        public IActionResult ActionPaymentPercentage([FromBody] JObject content)
        {
            string userObjectID = (User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value;

            JObject response;

            var action_details = content["actionFields"];

            if (action_details == null)
            {
                return StatusCode(400, ErrorMessages.ActionFliedsAreMissing());
            }

            var origin = action_details["origin"];
            var recipient = action_details["recipient"];
            var recipient_iban = action_details["recipient_iban"];
            var percentage = action_details["percentage"];
            var description = action_details["description"];

            if (recipient == null)
            {
                recipient = (User.FindFirst("name"))?.Value;
            }

            if (
                origin == null ||
                recipient == null ||
                recipient_iban == null ||
                percentage == null ||
                description == null
            )
            {
                return StatusCode(400, ErrorMessages.MandatoryFieldAreMissing());
            }

            string identifier = Guid.NewGuid().ToString();

            JObject details = new JObject {
                {"payment", new JObject {
                    {"origin", new JObject {
                        {"id", origin}
                    }},
                    {"recipient", new JObject {
                        {"name", recipient},
                        {"iban", recipient_iban}
                    }},
                    {"amount", new JObject {
                        {"type", "percent"},
                        {"value", percentage}
                    }},
                    {"description", description}
                }}
            };

            response = new JObject {
                {"data", new JArray {
                        new JObject {
                            {"id", identifier}
                        }
                    }
                }
            };

            if (Request.Headers["IFTTT-Test-Mode"].Equals("1"))
            {
                response.Add("metadata", new JObject {
                    {"paymentid", 0}
                });

                return StatusCode(200, response);
            }
            else
            {
                var paymentResponse = Payment.Execute(userObjectID, (JObject)details);

                response.Add("metadata", new JObject {
                    {"paymentid", paymentResponse}
                });

                return StatusCode(200, response);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("transfer_full_saldo")]
        [Route("transfer_full_saldo_other_account")]
        public IActionResult TransferFullSaldo([FromBody] JObject content)
        {
            string userObjectID = (User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value;

            JObject response;

            var action_details = content["actionFields"];

            if (action_details == null)
            {
                return StatusCode(400, ErrorMessages.ActionFliedsAreMissing());
            }

            var origin = action_details["origin"];
            var recipient = action_details["recipient"];
            var recipient_iban = action_details["recipient_iban"];
            var description = action_details["description"];

            if (recipient == null)
            {
                recipient = (User.FindFirst("name"))?.Value;
            }

            if (
                origin == null ||
                recipient == null ||
                recipient_iban == null ||
                description == null
            )
            {
                return StatusCode(400, ErrorMessages.MandatoryFieldAreMissing());
            }

            string identifier = Guid.NewGuid().ToString();

            JObject details = new JObject {
                {"payment", new JObject {
                    {"origin", new JObject {
                        {"id", origin}
                    }},
                    {"recipient", new JObject {
                        {"name", recipient},
                        {"iban", recipient_iban}
                    }},
                    {"amount", new JObject {
                        {"type", "percent"},
                        {"value", "100"}
                    }},
                    {"description", description}
                }}
            };

            response = new JObject {
                {"data", new JArray {
                        new JObject {
                            {"id", identifier}
                        }
                    }
                }
            };

            if (Request.Headers["IFTTT-Test-Mode"].Equals("1"))
            {
                response.Add("metadata", new JObject {
                    {"paymentid", 0}
                });

                return StatusCode(200, response);
            }
            else
            {
                var paymentResponse = Payment.Execute(userObjectID, (JObject)details);

                response.Add("metadata", new JObject {
                    {"paymentid", paymentResponse}
                });

                return StatusCode(200, response);
            }
        }

        //OptionLists
        [Authorize]
        [HttpPost]
        [Route("request_email/fields/origin/options")]
        [Route("request_iban/fields/origin/options")]
        [Route("transfer_amount/fields/origin/options")]
        [Route("transfer_percentage/fields/origin/options")]
        [Route("transfer_full_saldo/fields/origin/options")]
        [Route("transfer_amount_other_account/fields/origin/options")]
        [Route("transfer_percentage_other_account/fields/origin/options")]
        [Route("transfer_full_saldo_other_account/fields/origin/options")]
        public IActionResult ListAccountsRW()
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
                        if (account.AccessRights == "read/write")
                        {
                            ((JArray)response.GetValue("data")).Add(new JObject(
                                new JProperty("label", account.Description),
                                new JProperty("value", account.Id)
                            ));
                        }
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

        [Authorize]
        [HttpPost]
        [Route("transfer_amount_other_account/fields/recipient_iban/options")]
        [Route("transfer_percentage_other_account/fields/recipient_iban/options")]
        [Route("transfer_full_saldo_other_account/fields/recipient_iban/options")]
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