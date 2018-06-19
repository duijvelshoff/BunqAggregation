using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using bunqAggregation.Core;

namespace bunqAggregation.Intergration.IFTTT
{
    [Route("ifttt/v1/[controller]")]
    public class UserController : Controller
    {
        [Authorize(ActiveAuthenticationSchemes = Config.Client.Service)]
        [HttpGet]
        [Route("info")]
        public IActionResult Get()
        {
            var http = new HttpClient();
            http.DefaultRequestHeaders.Add("Authorization", Request.Headers["Authorization"].ToString());
            JObject userObject = JObject.Parse((http.GetStringAsync(Core.Config.Authority + "/connect/userinfo")).Result);

            JObject response = new JObject
            {
                {"data", new JObject{
                    {"name", userObject["name"]},
                    {"id", userObject["sub"]}
                }}
            };
            return StatusCode(200, response);
        }
    }
}