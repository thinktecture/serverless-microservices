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
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace Serverless
{
    public class GetSignalRConfigurationFunctions
    {
        [FunctionName("GetOrdersHubSignalRConfguration")]
        public async Task<IActionResult> GetOrdersHubSignalRConfguration(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "api/config/ordersHub")]
            HttpRequest req,

            [SignalRConnectionInfo(HubName = "ordersHub", ConnectionStringSetting="SignalR")]
            SignalRConnectionInfo connectionInfo,

            ILogger log)
        {
            log.LogInformation("GetOrdersHubSignalRConfguration HTTP trigger function processed a request.");

            if (!await req.CheckAuthorization("api"))
            {
                return new UnauthorizedResult();
            }

            return new OkObjectResult(connectionInfo);
        }

        [FunctionName("GetShippingsHubSignalRConfguration")]
        public async Task<IActionResult> GetShippingsHubSignalRConfguration(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "api/config/shippingsHub")]
            HttpRequest req,

            [SignalRConnectionInfo(HubName = "shippingsHub", ConnectionStringSetting="SignalR")]
            SignalRConnectionInfo connectionInfo,

            ILogger log)
        {
            log.LogInformation("GetShippingsHubSignalRConfguration HTTP trigger function processed a request.");

            if (!await req.CheckAuthorization("api"))
            {
                return new UnauthorizedResult();
            }

            return new OkObjectResult(connectionInfo);
        }
    }
}
