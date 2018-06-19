using System;
using System.Threading;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using MongoDB.Bson;
using Bunq.Sdk.Context;
using Bunq.Sdk.Model.Generated.Endpoint;
using bunqAggregation.Core;

namespace bunqAggregation.Intergration.bunq
{
    public class Account
    {
        public int Id { get; set; }
        public string AccessRights { get; set; }
        public string IBAN { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public double Balance { get; set; }

        public static Account Get(string IBAN = null, int Id = 0)
        {
            var ibanRegex = new Regex("^([A-Za-z]{2}[0-9]{2})(?=(?:[ ]?[A-Za-z0-9]){10,30}$)((?:[ ]?[A-Za-z0-9]{3,5}){2,6})([ ]?[A-Za-z0-9]{1,3})?$");
              
            if (IBAN != null && Id == 0)
            {
                var allMonetaryAccounts = MonetaryAccountBank.List().Value;

                foreach (var monetaryAccount in allMonetaryAccounts)
                {
                    if (monetaryAccount.Status == "ACTIVE")
                    {
                        foreach (var alias in monetaryAccount.Alias)
                        {
                            if (IBAN == alias.Value)
                            {
                                var account = new Account
                                {
                                    Id = (int)monetaryAccount.Id,
                                    IBAN = alias.Value,
                                    Description = monetaryAccount.Description,
                                    AccessRights = (monetaryAccount.Balance != null) ? "read/write" : "read-only",
                                    Balance = (monetaryAccount.Balance != null) ? Double.Parse(monetaryAccount.Balance.Value) : 0,
                                    UserId = (int)monetaryAccount.UserId
                                };
                                return (account);
                            }
                        }
                    }
                }
            }
            else if (IBAN == null && Id != 0)
            {
                var monetaryAccount = MonetaryAccountBank.Get(Id).Value;

                if (monetaryAccount.Status == "ACTIVE")
                {
                    foreach (var alias in monetaryAccount.Alias)
                    {
                        if (ibanRegex.IsMatch(alias.Value))
                        {
                            Account account = new Account
                            {
                                Id = (int)monetaryAccount.Id,
                                IBAN = alias.Value,
                                Description = monetaryAccount.Description,
                                AccessRights = (monetaryAccount.Balance != null) ? "read/write" : "read-only",
                                Balance = (monetaryAccount.Balance != null) ? Double.Parse(monetaryAccount.Balance.Value) : 0,
                                UserId = (int)monetaryAccount.UserId
                            };
                            return (account);
                        }
                    }
                }
            }
            return null;
        }

        public static bool Allowed(string UserId, int AccountId)
        {
            List<int> bunqIds = new List<int>();
            var filter = new BsonDocument("id", UserId);

            try
            {
                var userDocument = Collection.RetrieveDocument(filter);

                foreach (var bunqId in (BsonArray)userDocument["bunq"])
                {
                    bunqIds.Add(bunqId["userid"].AsInt32);
                }

                var monetaryAccount = MonetaryAccountBank.Get(AccountId).Value;

                return (bunqIds.Contains((int)monetaryAccount.UserId) && monetaryAccount.Status == "ACTIVE");
            }
            catch
            {
                return false;
            }
        }

        public static List<Account> List(string UserId)
        {
            List<int> bunqIds = new List<int>();
            List<Account> result = new List<Account>();

            Regex ibanRegex = new Regex("^([A-Za-z]{2}[0-9]{2})(?=(?:[ ]?[A-Za-z0-9]){10,30}$)((?:[ ]?[A-Za-z0-9]{3,5}){2,6})([ ]?[A-Za-z0-9]{1,3})?$");

            var filter = new BsonDocument("id", UserId);
            var userDocument = Collection.RetrieveDocument(filter);

            foreach (var bunqId in (BsonArray)userDocument["bunq"])
            {
                bunqIds.Add(bunqId["userid"].AsInt32);
            }

            var allMonetaryAccounts = MonetaryAccountBank.List().Value;

            foreach (var monetaryAccount in allMonetaryAccounts)
            {
                if (monetaryAccount.Status == "ACTIVE")
                {
                    if (bunqIds.Contains((int)monetaryAccount.UserId))
                    {
                        foreach (var alias in monetaryAccount.Alias)
                        {
                            if (ibanRegex.IsMatch(alias.Value))
                            {
                                Account account = new Account
                                {
                                    Id = (int)monetaryAccount.Id,
                                    IBAN = alias.Value,
                                    Description = monetaryAccount.Description,
                                    AccessRights = (monetaryAccount.Balance != null) ? "read/write" : "read-only",
                                    Balance = (monetaryAccount.Balance != null) ? Double.Parse(monetaryAccount.Balance.Value) : 0,
                                    UserId = (int)monetaryAccount.UserId
                                };
                                result.Add(account);
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}