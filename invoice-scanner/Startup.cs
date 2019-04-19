using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serverless;
using Willezone.Azure.WebJobs.Extensions.AzureKeyVault;

[assembly: WebJobsStartup(typeof(Startup))]

namespace Serverless
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var tempProvider = builder.Services.BuildServiceProvider();
            var config = tempProvider.GetRequiredService<IConfiguration>();

            // NOTE: This is *only* for local dev & demos - as we now have a secret in our code... again ;-(
            // For production use a MSI
            builder.AddAzureKeyVault(config["KeyVaultUrl"], config["KeyVaultClientId"], config["KeyVaultClientSecret"]);
        }
    }
}