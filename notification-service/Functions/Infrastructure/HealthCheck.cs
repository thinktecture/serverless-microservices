using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Serverless
{
    public class HealthCheck
    {
        [FunctionName("Ping")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "api/ping")]
            HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# Ping trigger function processed a request.");

            return new OkObjectResult("OK");
        }
    }
}
