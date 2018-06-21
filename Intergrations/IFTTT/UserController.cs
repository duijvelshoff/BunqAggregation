using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using bunqAggregation.Core;

namespace bunqAggregation.Intergrations.IFTTT
{
    [Route("ifttt/v1/[controller]")]
    public class UserController : Controller
    {
        [Authorize]
        [HttpGet]
        [Route("info")]
        public IActionResult Get()
        {
            string userObjectID = (User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value;
            string userObjectName = (User.FindFirst("name"))?.Value;

            JObject response = new JObject
            {
                {"data", new JObject{
                    {"name", userObjectName},
                    {"id", userObjectID}
                }}
            };
            return StatusCode(200, response);
        }
    }
}