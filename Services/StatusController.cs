using bunqAggregation.Core;
using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc;

namespace bunqAggregation.Services
{
    [Route("api/[controller]")]
    public class StatusController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            var jobDocument = Collection.RetrieveDocument(new BsonDocument("trigger", "job"));
            return Ok("Operational!\n\nLast time job ran: " + jobDocument["lastrun"].ToString());
        }
    }
}