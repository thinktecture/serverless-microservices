using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Serverless
{
    public static class UploadDocument
    {
        [FunctionName("UploadDocument")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST", Route = "documents")] 
            HttpRequestMessage req,

            [Blob("invoices", FileAccess.Write)]
            CloudBlobContainer blobContainer,

            ILogger log)
        {
            var provider = new MultipartMemoryStreamProvider();
            await req.Content.ReadAsMultipartAsync(provider);
            
            var file = provider.Contents.First();

            if(file == null)
            {
                return new BadRequestResult();
            }

            var fileInfo = file.Headers.ContentDisposition;

            if(fileInfo == null)
            {
                return new BadRequestResult();
            }

            var fileName = fileInfo.FileName.Replace("\"", "");
            var blob = blobContainer.GetBlockBlobReference(fileName);
            await blob.UploadFromStreamAsync(await file.ReadAsStreamAsync());
            
            return new OkResult();
        }
    }
}
