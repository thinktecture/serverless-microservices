using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Newtonsoft.Json.Linq;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure.KeyVault;
using Willezone.Azure.WebJobs.Extensions.AzureKeyVault;
using System.Threading;
using Nito.AsyncEx;

namespace Serverless
{
    public static class RecognizeTextFromImageDocument
    {
        private static AsyncLock mutex = new AsyncLock();
        private static OcrClient ocrClient;

        [FunctionName("RecognizeTextFromImage")]
        public static async Task Run(
            [EventGridTrigger]
            EventGridEvent eventGridEvent,

            [Blob("invoice-texts", FileAccess.Read)]
            CloudBlobContainer blobContainer,

            [AzureKeyVaultClient]
            IKeyVaultClient keyVaultClient,

            ILogger log)
        {
            log.LogInformation("RecognizeTextFromImage trigger function processed a request.");

            using (await mutex.LockAsync())
            {            
                if (ocrClient == null)
                {
                    // SEE https://github.com/BorisWilhelms/azure-function-keyvault/issues/1
                    var cognitiveServicesEndpoint = (await keyVaultClient.GetSecretAsync(Environment.GetEnvironmentVariable("KeyVaultUrl") ,"CognitiveServicesEndpoint")).Value;
                    var cognitiveServicesSubscriptionKey = (await keyVaultClient.GetSecretAsync(Environment.GetEnvironmentVariable("KeyVaultUrl") ,"CognitiveServicesSubscriptionKey")).Value;
                    
                    ocrClient = new OcrClient(
                        cognitiveServicesEndpoint, 
                        cognitiveServicesSubscriptionKey
                    );
                }
            }

            var createdEvent = ((JObject)eventGridEvent.Data).ToObject<StorageBlobCreatedEventData>();

            var fileUri = new Uri(createdEvent.Url);
            var fileName = Path.GetFileName(fileUri.LocalPath);

            var result = await ocrClient.RecognizeText(fileUri.ToString());
            var response = result.Lines.Select(l => new { Text = l.Text }).ToList();

            var blobName = fileName.Replace("png", "txt");
            var blob = blobContainer.GetBlockBlobReference(blobName);

            await blob.UploadTextAsync(JsonConvert.SerializeObject(response));
        }
    }
}
