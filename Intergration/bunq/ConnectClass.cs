using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Bunq.Sdk.Context;
using Bunq.Sdk.Model.Generated.Object;
using Bunq.Sdk.Model.Generated.Endpoint;
using System.Text.RegularExpressions;
using bunqAggregation.Core;
using MongoDB.Bson;

namespace bunqAggregation.Intergration.bunq
{
    public static class Connect
    {
        public class Create
        {
            public int Id { get; }
            public string QRCode { get; }

            public Create(string UserId)
            {
                var currentDate = DateTime.UtcNow;
                var expirationTime = currentDate.AddMinutes(1).ToString("yyyy-MM-dd HH:mm:ss");
                var draftShareInviteEntry = new DraftShareInviteEntry(new ShareDetail { Payment = new ShareDetailPayment(true, true, true, true) });

                Id = DraftShareInviteBank.Create(expirationTime, draftShareInviteEntry).Value;
                QRCode = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(DraftShareInviteBankQrCodeContent.List(Id).Value));

                Task.Run(() => Status(UserId, Id, true));
            }
        }

        public static JObject Status(string UserId, int DraftId, bool IsTask = false)
        {
            JObject response = new JObject();

            string accountDetailsIban = "";
            double counter = 0;
            double timeout = 60;

            JObject result = JObject.Parse(Convert.ToString(DraftShareInviteBank.Get(DraftId).Value));
            while ((string)result["status"] == "PENDING")
            {
                Thread.Sleep(5000);
                counter += 5;
                if (counter == timeout)
                {
                    Console.WriteLine(UserId + " : " + DraftId + " : Connect with Currect Account was timed-out.");
                    response.Add("error", new JObject{
                        { "message", "Connect with Currect Account was timed-out."}
                    });
                    return response;
                }
                result = JObject.Parse(Convert.ToString(DraftShareInviteBank.Get(DraftId).Value));
            }
            if ((string)result["status"] == "USED")
            {
                result = JObject.Parse(Convert.ToString(ShareInviteBankResponse.Get(Convert.ToInt32((string)result["share_invite_bank_response_id"])).Value));
                if ((string)result["status"] == "ACCEPTED")
                {
                    var accountDetails = MonetaryAccountBank.Get(Convert.ToInt32((string)result["monetary_account_id"])).Value;
                    Regex ibanRegex = new Regex("^([A-Za-z]{2}[0-9]{2})(?=(?:[ ]?[A-Za-z0-9]){10,30}$)((?:[ ]?[A-Za-z0-9]{3,5}){2,6})([ ]?[A-Za-z0-9]{1,3})?$");

                    foreach (var alias in accountDetails.Alias)
                    {
                        if (ibanRegex.IsMatch(alias.Value))
                        {
                            accountDetailsIban = alias.Value;
                        }
                    }
                    if(IsTask)
                    {
                        var filter = new BsonDocument("id", UserId);
                        var userDocument = Collection.RetrieveDocument(filter);

                        if (userDocument != null)
                        {
                            var updateDocument = new BsonDocument {
                            { "$addToSet", new BsonDocument {
                                {"bunq", new BsonDocument {
                                    {"userid", accountDetails.UserId}
                                }}
                            }}
                        };
                            Collection.UpdateDocument(filter, updateDocument);
                        }
                        else
                        {
                            var updateDocument = new BsonDocument {
                                {"id", UserId},
                                {"bunq", new BsonArray {
                                    new BsonDocument {
                                        {"userid", accountDetails.UserId}
                                    }
                                }}
                            };
                            Collection.CreateDocument(updateDocument);
                        }   
                    }
                    Console.WriteLine(UserId + " : " + DraftId + " : Connected with : " + accountDetails.Id + "(" + accountDetails.UserId + ")");
                    response.Add("data", new JObject{
                        {"description", accountDetails.Description},
                        {"iban", accountDetailsIban}
                    });
                    return response;
                }
                else
                {
                    Console.WriteLine(UserId + " : " + DraftId + " : Connect with Currect Account was unsuccessful.");
                    response.Add("error", new JObject{
                        {"message", "Connect with Currect Account was unsuccessful."}
                    });
                    return response;
                }
            }
            else
            {
                Console.WriteLine(UserId + " : " + DraftId + " : Connect with Currect Account was timed-out.");
                response.Add("error", new JObject{
                        { "message", "Connect with Currect Account was timed-out."}
                    });
                return response;
            }
        }
    }
}