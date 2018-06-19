using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace bunqAggregation.Core
{
    public class ErrorController : Controller
    {
        [HttpGet("/Error"), HttpPost("/Error")]
        public IActionResult Error([FromQuery] int status = 400)
        {
            JObject response;

            if (status == 401)
            {
                return StatusCode(status, ErrorMessages.AuthenticationFailed());
            }
            else
            {
                response = new JObject
                {
                    {"errors", new JArray {
                            new JObject {
                                {"message", "Unkown error."}
                            }
                        }
                    }
                };
                return StatusCode(status, response);
            }
        }
    }

    public class ErrorMessages
    {
        public static JObject AuthenticationFailed()
        {
            JObject response = new JObject
            {
                {"errors", new JArray {
                        new JObject {
                            {"message", "Authentication unsuccessful."}
                        }
                    }
                }
            };
            return response;
        }

        public static JObject NoServiceKeyOrChannelKey()
        {
            JObject response = new JObject
            {
                {"errors", new JArray {
                        new JObject {
                            {"message", "Invalid channel or service key."}
                        }
                    }
                }
            };
            return response;
        }

        public static JObject ActionFliedsAreMissing()
        {
            JObject response = new JObject
            {
                {"errors", new JArray {
                        new JObject {
                            {"message", "Whoops, the actionFields are missing!"}
                        }
                    }
                }
            };
            return response;
        }

        public static JObject TriggerIdentityIsMissing()
        {
            JObject response = new JObject
            {
                {"errors", new JArray {
                        new JObject {
                            {"message", "Whoops, the trigger_identity is missing!"}
                        }
                    }
                }
            };
            return response;
        }

        public static JObject TriggerFieldsAreMissing()
        {
            JObject response = new JObject
            {
                {"errors", new JArray {
                        new JObject {
                            {"message", "Whoops, the triggerFields are missing!"}
                        }
                    }
                }
            };
            return response;
        }

        public static JObject DataObjectIsMissing()
        {
            JObject response = new JObject
            {
                {"error", new JObject {
                        {"message", "Whoops, the data object is missing!"}
                    }
                }
            };
            return response;
        }

        public static JObject MandatoryFieldAreMissing()
        {
            JObject response = new JObject
        {
            {"errors", new JArray {
                    new JObject {
                        {"message", "Whoops, one of the mandatory fields are missing!"}
                    }
                }
            }
        };
            return response;
        }

        public static JObject NoIbanAndMailTogether()
        {
            JObject response = new JObject
        {
            {"error", new JObject {
                    {"message", "Whoops, you cant use iban and mail together!"}
                }
            }
        };
            return response;
        }
    }
}
