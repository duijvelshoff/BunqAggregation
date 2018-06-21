using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using bunqAggregation.Core;
using System.Net.Http;
using MongoDB.Bson;

namespace bunqAggregation.Services
{
    [Route("api/[controller]")]
    public class RulesController : Controller
    {
        [Authorize]
        [Route("list")]
        [HttpGet]
        public IActionResult List()
        {
            string userObjectID = (User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value;

            JObject response = new JObject();
            JObject rules = new JObject();

            if (Collection.Registerd(userObjectID))
            {
                List<Rule> Rules = Rule.List(userObjectID);
                if (Rules.Count > 0)
                {
                    rules.Add("rules", new JArray());
                    foreach (Rule Rule in Rules)
                    {
                        ((JArray)rules.GetValue("rules")).Add(new JObject{
                            {"id", Rule.Id},
                            {"name", Rule.Name},
                            {"condition", Rule.Condition},
                            {"actions", Rule.Actions}
                        });
                    }
                    response.Add("data", rules);
                }
                else
                {
                    response.Add("error", new JObject {
                        {"message", "The user has no rules active!"}
                    });
                }
            }
            else
            {
                response.Add("error", new JObject {
                    {"message", "The user has no bunq Current Account added yet!"}
                });
            }

            return StatusCode(200, response);
        }

        [Authorize]
        [Route("add")]
        [HttpPost]
        public IActionResult Add([FromBody] JObject content)
        {
            JObject response = new JObject();

            return StatusCode(200, content);
        }

        [Authorize]
        [Route("update")]
        [HttpPost]
        public IActionResult Update([FromBody] JObject content)
        {
            JObject response = new JObject();

            return StatusCode(200, content);
        }

        [Authorize]
        [Route("delete")]
        [HttpPost]
        public IActionResult Delete([FromBody] JObject content)
        {
            JObject response = new JObject();

            return StatusCode(200, content);
        }
    }
}
