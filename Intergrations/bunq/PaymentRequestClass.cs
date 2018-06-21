using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json.Linq;
using Bunq.Sdk.Context;
using Bunq.Sdk.Model.Generated.Endpoint;
using Bunq.Sdk.Model.Generated.Object;

namespace bunqAggregation.Intergrations.bunq
{
    public class PaymentRequest
    {
        public class Details
        {
            public Account Origin { get; }
            public Amount Amount { get; }
            public Pointer Recipient { get; }
            public string Description { get; }

            public Details(JObject request)
            {

                var origin_iban = request["request"]["origin"]["iban"];
                var origin_id = request["request"]["origin"]["id"];

                if (origin_iban != null)
                {
                    Origin = Account.Get((string)request["request"]["origin"]["iban"],0);
                }
                if (origin_id != null)
                {
                    Origin = Account.Get(null,(int)request["request"]["origin"]["id"]);
                }

                Amount = new Amount((string)request["request"]["amount"]["value"], "EUR");

                Regex emailRegex = new Regex("^[_a-z0-9-]+(.[a-z0-9-]+)@[a-z0-9-]+(.[a-z0-9-]+)*(.[a-z]{2,4})$");
                Regex ibanRegex = new Regex("^([A-Za-z]{2}[0-9]{2})(?=(?:[ ]?[A-Za-z0-9]){10,30}$)((?:[ ]?[A-Za-z0-9]{3,5}){2,6})([ ]?[A-Za-z0-9]{1,3})?$");

                if (emailRegex.IsMatch((string)request["request"]["recipient"]["value"]))
                {
                    Recipient = new Pointer("EMAIL", (string)request["request"]["recipient"]["value"]);
                }
                if (ibanRegex.IsMatch((string)request["request"]["recipient"]["value"]))
                {
                    Recipient = new Pointer("IBAN", (string)request["request"]["recipient"]["value"]);
                    Recipient.Name = (string)request["request"]["recipient"]["name"];
                }

                DateTime now = DateTime.Today;
                string month = now.ToString("MMMM", new CultureInfo("nl-NL"));
                string year = now.ToString("yyyy");
                var descrtiption = (string)request["request"]["description"];
                if (descrtiption != null)
                {
                    Description = String.Format((string)request["request"]["description"], month, year);
                }
                else
                {
                    Description = "";
                }
            }
        }

        public static int Execute(string UserId, JObject request, bool IsTaks = false)
        {
            Details requestDetails = new Details(request);
            if (requestDetails.Origin.AccessRights == "read/write")
            {
                try
                {
                    return Bunq.Sdk.Model.Generated.Endpoint.RequestInquiry.Create(requestDetails.Amount, requestDetails.Recipient, requestDetails.Description, true, requestDetails.Origin.Id).Value;
                }
                catch
                {
                    Thread.Sleep(3000);
                    return Bunq.Sdk.Model.Generated.Endpoint.RequestInquiry.Create(requestDetails.Amount, requestDetails.Recipient, requestDetails.Description, true, requestDetails.Origin.Id).Value;
                }
            }
            else
            {
                return 0;
            }
        }
    }
}
