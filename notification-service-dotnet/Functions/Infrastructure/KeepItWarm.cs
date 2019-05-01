using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Serverless
{
    public class KeepItWarm
    {
        [FunctionName("KeepItWarm")]
        public void Run(
            [TimerTrigger("0 */9 * * * *", RunOnStartup = true)]
            TimerInfo myTimer,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}
