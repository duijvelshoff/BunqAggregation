using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using IdentityModel.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using bunqAggregation.Models;
using bunqAggregation.Core;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace bunqAggregation.WebApplication
{
    public class ManageController : Controller
    {
        [Authorize(ActiveAuthenticationSchemes = Config.Client.Browser)]
        public async Task<IActionResult> Index()
        {
            var accessToken = await HttpContext.Authentication.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.SetBearerToken(accessToken);
            try
            {
                JObject result = JObject.Parse((await client.GetStringAsync(Core.Config.Service.Url + "/api/accounts/list")));

                ViewData["accountCount"] = (((JArray)result["data"]["accounts"]).Count).ToString();

                List<CurrentAccount> currentAccounts = new List<CurrentAccount>();

                foreach (var currentAccount in (JArray)result["data"]["accounts"])
                {
                    currentAccounts.Add(new CurrentAccount
                    {
                        Id = currentAccount["id"].ToString(),
                        AccessRights = currentAccount["access_rights"].ToString(),
                        Description = currentAccount["description"].ToString(),
                        IBAN = currentAccount["iban"].ToString()
                    });
                }

                var model = new CurrentAccountsModel
                {
                    CurrentAccounts = currentAccounts
                };

                return View(model);
            }
            catch
            {
                return RedirectToAction(nameof(ManageController.AddCurrentAccount));
            }
        }

        [Authorize(ActiveAuthenticationSchemes = Config.Client.Browser)]
        public async Task<IActionResult> AddCurrentAccount()
        {
            var accessToken = await HttpContext.Authentication.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.SetBearerToken(accessToken);
            JObject response = JObject.Parse((await client.GetStringAsync(Core.Config.Service.Url + "/api/accounts/add")));

            ViewData["access_token"] = accessToken;
            var model = new AddCurrentAccountModel
            {
                QRCode = response["data"]["qrcode"].ToString(),
                DraftId = response["data"]["draftid"].ToString()
            };
            return View(model);
        }
    }
}
