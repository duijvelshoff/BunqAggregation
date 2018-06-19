using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using bunqAggregation.Core;
using bunqAggregation.Intergration.bunq;

namespace bunqAggregation.Services
{
    [Route("api/[controller]")]
    public class PaymentController : Controller
    {
        [Authorize(ActiveAuthenticationSchemes = Config.Client.Service)]
        [HttpPost]
        public IActionResult Execute([FromBody] JObject content)
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

            //TODO: Build MandatoryFieldValidation
            if (false)
            {
                return StatusCode(400, ErrorMessages.MandatoryFieldAreMissing());
            }

            var paymentResponse = Payment.Execute(UserId, (JObject)data);

            if (paymentResponse != 0)
            {
                response.Add("data", new JObject {
                    {"paymentid", paymentResponse}
                });
            }
            else
            {
                response.Add("error", new JObject{
                    {"message", "Payment was unsuccessful."}
                });
            }

            return StatusCode(200, response);
        }
    }
}