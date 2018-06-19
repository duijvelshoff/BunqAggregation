using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using bunqAggregation.Core;
using bunqAggregation.Intergration.bunq;

namespace bunqAggregation.Services
{
    [Route("api/[controller]")]
    public class RequestController : Controller
    {
        [Authorize(ActiveAuthenticationSchemes = Config.Client.Service)]
        [HttpPost]
        public IActionResult Create([FromBody] JObject content)
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

            var data = content["data"];

            if (data == null)
            {
                return StatusCode(400, ErrorMessages.DataObjectIsMissing());
            }

            var origin = data["request"]["origin"]["iban"];
            var recipient_name = data["request"]["recipient"]["name"];
            var recipient_mail = data["request"]["recipient"]["mail"];
            var recipient_iban = data["request"]["recipient"]["iban"];

            if (recipient_mail != null && recipient_iban !=null)
            {
                return StatusCode(400, ErrorMessages.NoIbanAndMailTogether());
            }

            if (recipient_mail != null)
            {
                ((JObject)data["request"]["recipient"]).Add("value",recipient_mail);
            }

            if (recipient_iban != null)
            {
                ((JObject)data["request"]["recipient"]).Add("value", recipient_iban);
            }

            var recipient_value = data["request"]["recipient"]["value"];
            var amount = data["request"]["amount"]["value"];
            var description = data["request"]["description"];

            if (
                origin  == null ||
                recipient_name == null ||
                recipient_value == null ||
                amount == null ||
                description == null
            )
            {
                return StatusCode(400, ErrorMessages.MandatoryFieldAreMissing());
            }

            var requestResponse = PaymentRequest.Execute(UserId, (JObject)data);

            if (requestResponse != 0)
            {
                response.Add("data", new JObject {
                    {"requestid", requestResponse}
                });
            }
            else
            {
                response.Add("error", new JObject{
                    {"message", "Request was unsuccessful."}
                });
            }

            return StatusCode(200, response);
        }
    }
}