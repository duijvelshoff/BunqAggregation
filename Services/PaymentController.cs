﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using bunqAggregation.Core;
using bunqAggregation.Intergrations.bunq;

namespace bunqAggregation.Services
{
    [Route("api/[controller]")]
    public class PaymentController : Controller
    {
        [Authorize]
        [HttpPost]
        public IActionResult Execute([FromBody] JObject content)
        {
            string userObjectID = (User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value;

            JObject response = new JObject();

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

            var paymentResponse = Payment.Execute(userObjectID, (JObject)data);

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