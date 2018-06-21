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

            var response = new JObject();
            var rules = new JObject();

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
            string userObjectID = (User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value;

            var response = new JObject();

            var data = content["data"];

            Rule rule = new Rule
            {
                Name = (string)data["rule"]["name"],
                Condition = (JObject)data["rule"]["condition"],
                Actions = (JArray)data["rule"]["actions"]
            };

            var ruleId = Rule.Add(userObjectID, rule);

            response.Add("data", new JObject {
                {"ruleid", ruleId}
            });

            return StatusCode(200, response);
        }

        [Authorize]
        [Route("update")]
        [HttpPost]
        public IActionResult Update([FromBody] JObject content)
        {
            string userObjectID = (User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value;

            var response = new JObject();

            var data = content["data"];

            Rule rule = new Rule
            {
                Id = (string)data["rule"]["id"],
                Name = (string)data["rule"]["name"],
                Condition = (JObject)data["rule"]["condition"],
                Actions = (JArray)data["rule"]["actions"]
            };

            var update = Rule.Update(userObjectID, rule);

            response.Add("data", update);

            return StatusCode(200, response);
        }

        [Authorize]
        [Route("delete")]
        [HttpPost]
        public IActionResult Delete([FromBody] JObject content)
        {
            string userObjectID = (User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier"))?.Value;

            var data = content["data"];

            Rule rule = new Rule
            {
                Id = (string)data["rule"]["id"]
            };

            var delete = new JObject();
            try
            {
                Rule.Delete(userObjectID, rule.Id);
                delete.Add("status", "executed");
            }
            catch
            {
                delete.Add("error", new JObject{
                    {"message", "Execution failed!"}
                });
            }

            return StatusCode(200, delete);
        }
    }
}
