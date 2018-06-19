using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using IdentityModel.Client;
using bunqAggregation.Core;

namespace bunqAggregation.WebApplication
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(ActiveAuthenticationSchemes = Config.Client.Browser)]
        public IActionResult Secure()
        {
            ViewData["Message"] = "Secure page.";

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync("Cookies");
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        public IActionResult Error()
        {
            return View();
        }

        public async Task<IActionResult> CallApiUsingClientCredentials()
        {
            var tokenClient = new TokenClient(Config.Authority + "/connect/token", "bunqaggregation", Config.Service.Secret);
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("bunqaggregation");

            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);
            var content = await client.GetStringAsync(Config.Service.Url + "/identity");

            ViewBag.Json = JArray.Parse(content).ToString();
            return View("json");
        }

        public async Task<IActionResult> CallApiUsingUserAccessToken()
        {
            var accessToken = await HttpContext.Authentication.GetTokenAsync("access_token");

            var client = new HttpClient();
            client.SetBearerToken(accessToken);
            var content = await client.GetStringAsync(Config.Service.Url + "/identity");

            ViewBag.Json = JArray.Parse(content).ToString();
            return View("json");
        }
    }
}