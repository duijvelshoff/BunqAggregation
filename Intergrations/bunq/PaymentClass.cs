using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using Newtonsoft.Json.Linq;
using Bunq.Sdk.Context;
using Bunq.Sdk.Model.Generated.Object;
using Bunq.Sdk.Model.Generated.Endpoint;

namespace bunqAggregation.Intergrations.bunq
{
    public class Payment
    {
        public class Details
        {
            public Account Origin { get; }
            public Amount Amount { get; }
            public Pointer Recipient { get; }
            public string Description { get; }

            public Details(JObject request)
            {
                var origin_iban = request["payment"]["origin"]["iban"];
                var origin_id = request["payment"]["origin"]["id"];

                if(origin_iban != null)
                {
                    Origin = Account.Get((string)request["payment"]["origin"]["iban"],0);
                }
                if(origin_id != null)
                {
                    Origin = Account.Get(null,(int)request["payment"]["origin"]["id"]);
                }
                double AmountToTransfer = 0;
                double TransactionAmount = 0;

                var MetaData = request["metadata"];
                if (MetaData != null)
                {
                    TransactionAmount = Double.Parse((string)MetaData["callback"]["amount"]);
                }

                switch ((string)request["payment"]["amount"]["type"])
                {
                    case "exact":
                        if (Origin.Balance >= Double.Parse((string)request["payment"]["amount"]["value"]))
                        {
                            AmountToTransfer = Double.Parse((string)request["payment"]["amount"]["value"]);
                        }
                        else
                        {
                            AmountToTransfer = Origin.Balance;
                        }
                        break;
                    case "percent":
                        AmountToTransfer = Origin.Balance * (Double.Parse((string)request["payment"]["amount"]["value"]) / 100);
                        break;
                    case "differance":
                        if (TransactionAmount > 0)
                        {
                            AmountToTransfer = Origin.Balance - TransactionAmount;
                        }
                        break;
                    case "roundup":
                        if (TransactionAmount > 0)
                        {
                            AmountToTransfer = Math.Ceiling(TransactionAmount) - TransactionAmount;
                        }
                        break;
                    default:
                        break;
                }
                Amount = new Amount(AmountToTransfer.ToString("0.00"), "EUR");
                Recipient = new Pointer("IBAN", (string)request["payment"]["recipient"]["iban"]);
                Recipient.Name = (string)request["payment"]["recipient"]["name"];

                DateTime now = DateTime.Today;
                string month = now.ToString("MMMM", new CultureInfo("nl-NL"));
                string year = now.ToString("yyyy");

                Description = String.Format((string)request["payment"]["description"], month, year);
            }
        }

        public static int Execute(string UserId, JObject request)
        {
            Details paymentDetails = new Details(request);

            if (Double.Parse(paymentDetails.Amount.Value) > 0)
            {
                if (paymentDetails.Origin.AccessRights == "read/write")
                {
                    try
                    {
                        return Bunq.Sdk.Model.Generated.Endpoint.Payment.Create(paymentDetails.Amount, paymentDetails.Recipient, paymentDetails.Description, paymentDetails.Origin.Id).Value;
                    }
                    catch
                    {
                        Thread.Sleep(3000);
                        return Bunq.Sdk.Model.Generated.Endpoint.Payment.Create(paymentDetails.Amount, paymentDetails.Recipient, paymentDetails.Description, paymentDetails.Origin.Id).Value;
                    }
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
    }
}
