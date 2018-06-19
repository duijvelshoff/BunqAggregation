using System;
using System.Collections;
using System.Collections.Generic;

namespace bunqAggregation.Models
{
    public class CurrentAccount
    {
        public string Id { get; set; }
        public string AccessRights { get; set; }
        public string IBAN { get; set; }
        public string Description { get; set; }
    }
    public class CurrentAccountsModel
    {
        public List<CurrentAccount> CurrentAccounts { get; set; }
    }

    public class AddCurrentAccountModel
    {
        public string QRCode { get; set; }
        public string DraftId { get; set; }
    }
}