using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Aliencube.AzureFunctions.Extensions.OpenApi;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using Aliencube.AzureFunctions.Extensions.OpenApi.Configurations;
using Aliencube.AzureFunctions.Extensions.OpenApi.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;

namespace Serverless
{
    public class OpenApiFunctions
    {
        [FunctionName(nameof(RenderSwaggerDocument))]
        [OpenApiIgnore]
        public async Task<IActionResult> RenderSwaggerDocument(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "swagger.json")] HttpRequest req,
            ILogger log)
        {
            var settings = new AppSettings();
            var filter = new RouteConstraintFilter();
            var helper = new DocumentHelper(filter);
            var document = new Document(helper);

            var result = await document.InitialiseDocument()
                                       .AddMetadata(settings.OpenApiInfo)
                                       .AddServer(req, settings.HttpSettings.RoutePrefix)
                                       .Build(Assembly.GetExecutingAssembly())
                                       .RenderAsync(OpenApiSpecVersion.OpenApi2_0, OpenApiFormat.Json)
                                       .ConfigureAwait(false);

            var response = new ContentResult()
            {
                Content = result,
                ContentType = "application/json",
                StatusCode = (int)HttpStatusCode.OK
            };

            return response;
        }

        [FunctionName(nameof(RenderSwaggerUI))]
        [OpenApiIgnore]
        public static async Task<IActionResult> RenderSwaggerUI(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "swagger/ui")] HttpRequest req,
            ILogger log)
        {
            var settings = new AppSettings();
            var ui = new SwaggerUI();

            var result = await ui.AddMetadata(settings.OpenApiInfo)
                                 .AddServer(req, settings.HttpSettings.RoutePrefix)
                                 .BuildAsync()
                                 .RenderAsync("swagger.json", settings.SwaggerAuthKey)
                                 .ConfigureAwait(false);

            var response = new ContentResult()
            {
                Content = result,
                ContentType = "text/html",
                StatusCode = (int)HttpStatusCode.OK
            };

            return response;
        }
    }
}