using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Documents;
using Serverless.Messages;

namespace Serverless
{
    public class GetOrdersFunctions
    {
        [FunctionName("GetOrders")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "api/orders")]
            HttpRequest req,

            IBinder binder,

            ILogger log)
        {
            log.LogInformation("GetOrders HTTP trigger function processed a request.");

            /*
            if (!await req.CheckAuthorization("api"))
            {
                return new UnauthorizedResult();
            }
            */
            
            var cosmosDb = new CosmosDBAttribute("ordersservice", "data");
            cosmosDb.ConnectionStringSetting = "CosmosDB";
 
            var ordersData = await binder.BindAsync<IEnumerable<NewOrderMessage>>(cosmosDb);
            var orders = ordersData.Select(doc => doc.Order).OrderByDescending(o => o.Created).ToList();

            return new OkObjectResult(orders);
        }
    }
}
