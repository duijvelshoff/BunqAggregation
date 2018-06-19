using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using bunqAggregation.Core;
using bunqAggregation.Intergration.bunq;

namespace bunqAggregation.Services
{
    [Route("api/[controller]")]
    public class AccountsController : Controller
    {
        [Authorize(ActiveAuthenticationSchemes = Config.Client.Service)]
        [Route("list")]
        [HttpGet]
        public IActionResult List()
        {
            JObject response = new JObject();
            JObject accounts = new JObject();

            string UserId = null;

            foreach (var claim in User.Claims) {
                if (claim.Type == "sub")
                {
                    UserId = claim.Value;
                }
            }

            if (Collection.Registerd(UserId))
            {
                List<Account> Accounts = Account.List(UserId);
                if(Accounts.Count > 0)
                {
                    accounts.Add("accounts", new JArray());
                    foreach (Account Account in Accounts)
                    {
                        ((JArray)accounts.GetValue("accounts")).Add(new JObject{
                            {"id", Account.Id},
                            {"iban", Account.IBAN},
                            {"description", Account.Description},
                            {"access_rights", Account.AccessRights}
                        });
                    }
                    response.Add("data", accounts);
                }
                else
                {
                    response.Add("error", new JObject {
                        {"message", "The user has no bunq Current Accounts active!"}
                    });
                }
            }
            else
            {
                response.Add("error", new JObject {
                    {"message", "The user has no bunq Current Account added yet!"}
                });
            }

            return StatusCode(200,response);
        }

        [Authorize(ActiveAuthenticationSchemes = Config.Client.Service)]
        [Route("add")]
        [HttpGet]
        public IActionResult Add()
        {
            JObject response = new JObject();
            string UserId = null;

            foreach (var claim in User.Claims)
            {
                if (claim.Type == "sub")
                {
                    UserId = claim.Value;
                }
            }

            var Draft = new Connect.Create(UserId);

            response.Add("data", new JObject {
                {"draftid", Draft.Id},
                {"qrcode", Draft.QRCode}
            });

            return StatusCode(200, response);
        }

        [Authorize(ActiveAuthenticationSchemes = Config.Client.Service)]
        [Route("status")]
        [HttpPost]
        public IActionResult Status([FromBody] JObject content)
        {
            string UserId = null;
            foreach (var claim in User.Claims)
            {
                if (claim.Type == "sub")
                {
                    UserId = claim.Value;
                }
            }

            var data = content["data"];

            if (data == null)
            {
                return StatusCode(400, ErrorMessages.DataObjectIsMissing());
            }

            var draftid = data["draftid"];

            if (
                draftid == null
            )
            {
                return StatusCode(400, ErrorMessages.MandatoryFieldAreMissing());
            }

            int DraftId = Convert.ToInt32(draftid);

            var response = Connect.Status(UserId, DraftId);

            return StatusCode(200, response);
        }
    }
}