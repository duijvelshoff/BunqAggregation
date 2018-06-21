using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using bunqAggregation.Core;

namespace bunqAggregation.Intergrations.IFTTT
{
    [Route("ifttt/v1/[controller]")]
    public class StatusController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            bool channel_key = (Config.IFTTT.ChannelKey == Request.Headers["IFTTT-Channel-Key"]);
            bool service_key = (Config.IFTTT.ServiceKey == Request.Headers["IFTTT-Service-Key"]);

            if (channel_key && service_key)
            {
                return StatusCode(200,"Operational!");
            }
            else
            {
                return StatusCode(401, ErrorMessages.NoServiceKeyOrChannelKey());
            }
        }
    }
}