using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Bunq.Sdk.Context;
using Bunq.Sdk.Model.Generated.Endpoint;
using Bunq.Sdk.Model.Generated.Object;
using System.Threading;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace bunqAggregation.Services
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        // GET: api/values
        [HttpGet]
        public void Get()
        {
            double AmountToTransfer;
            double TransactionAmount = 3.18;
            double AmoundRound = Math.Round(TransactionAmount, 0);
            AmountToTransfer  = (TransactionAmount < AmoundRound) ? AmoundRound - TransactionAmount : AmoundRound + 1 - TransactionAmount;
            Console.WriteLine(AmountToTransfer);
        }
    }
}
