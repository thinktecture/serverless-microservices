using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Serverless
{
    public class LetsEncryptFunctions
    {
        [FunctionName("LetsEncrypt")]
        public HttpResponseMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = ".well-known/acme-challenge/{code}")]
            HttpRequest req, 
            string code, 
            ILogger log)
        {
            log.LogInformation($"LetsEncrypt HTTP trigger function processed a request: {code}");

            var content = File.ReadAllText(@"D:\home\site\LetsEncrypt\.well-known\acme-challenge\" + code);
            var resp = new HttpResponseMessage(HttpStatusCode.OK);
            resp.Content = new StringContent(content, Encoding.UTF8, "text/plain");

            return resp;
        }
    }
}