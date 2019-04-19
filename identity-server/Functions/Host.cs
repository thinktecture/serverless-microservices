using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;
using IdentityServer;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Serverless
{
    // Based on http://www.luckenuik.net/migrate-your-aspnet                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                -core-based-identityserver-inside-azure-functions/
    public static class Host
    {
        private static readonly TestServer Server;
        private static readonly HttpClient ServerHttpClient;

        static Host()
        {
            var functionPath = Path.Combine(new FileInfo(typeof(Host).Assembly.Location).Directory.FullName, "..");
            Environment.SetEnvironmentVariable("HOST_FUNCTION_CONTENT_PATH", functionPath, EnvironmentVariableTarget.Process);

            Server = new TestServer(new WebHostBuilder()
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    config
                        .SetBasePath(functionPath)
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{builderContext.HostingEnvironment.EnvironmentName}.json",
                            optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables();

                    var builtConfig = config.Build();

                    config.AddAzureKeyVault(
                        builtConfig["KeyVault:BaseUrl"],
                        builtConfig["KeyVault:ClientId"],
                        builtConfig["KeyVault:ClientSecret"]);
                })
                .UseApplicationInsights()
                .UseContentRoot(functionPath)
                .UseStartup<Startup>()
                .UseSerilog((context, configuration) =>
                {
                    configuration
                        .MinimumLevel.Debug()
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .MinimumLevel.Override("System", LogEventLevel.Warning)
                        .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                        .Enrich.FromLogContext()
                        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate)
                        .WriteTo.ApplicationInsightsTraces(System.Environment.GetEnvironmentVariable("APPINSIGHTS_INSTRUMENTATIONKEY"));
                }));

            ServerHttpClient = Server.CreateClient();
        }

        [FunctionName("AllPaths")]
        public static async Task<HttpResponseMessage> RunAllPaths(
            CancellationToken ct,
            [HttpTrigger(AuthorizationLevel.Anonymous, Route = "{*x:regex(^(?!admin|debug|runtime).*$)}")]
            HttpRequestMessage req,
            ILogger log,
            ExecutionContext ctx)
        {
            return await ServerHttpClient.SendAsync(req, ct);
        }

        [FunctionName("Root")]
        public static async Task<HttpResponseMessage> RunRoot(
            CancellationToken ct,
            [HttpTrigger(AuthorizationLevel.Anonymous, Route = "/")]
            HttpRequestMessage req,
            ILogger log,
            ExecutionContext ctx)
        {
            return await ServerHttpClient.SendAsync(req, ct);
        }
    }
}