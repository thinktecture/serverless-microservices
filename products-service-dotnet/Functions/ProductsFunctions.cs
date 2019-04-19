using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage.Table;
using System.Diagnostics;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;

namespace Serverless
{
    public static class ProductsFunctions
    {
        private const string ALLPRODUCTS = "allproducts";

        [FunctionName("ListProducts")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "products")]
            HttpRequest req,

            [Table("products", "products")]
            CloudTable productsTable,
            
            [Inject]
            IMemoryCache cache,
            
            ILogger log)
        {
            log.LogInformation("***ListProducts HTTP trigger function processed a request.");

            List<Product> products;

            if(!cache.TryGetValue(ALLPRODUCTS, out products))
            {
                var query = new TableQuery<ProductEntity>();
                var productEntities = (await productsTable.ExecuteQuerySegmentedAsync(query, null)).ToList();
                
                products  = productEntities.Select(p => p.ToProduct()).ToList();

                var expirationTime = DateTime.Today.AddDays(1);
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(expirationTime);
                cache.Set(ALLPRODUCTS, products, cacheEntryOptions);
            }

            return new OkObjectResult(products);
        }

        [FunctionName("GetProduct")]
        public static IActionResult GetProductById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "products/{id}")]
            HttpRequest req,
            
            [Table("products", "products", "{id}")]
            ProductDetailsEntity productDetailsEntity,
            
            string id,
            
            ILogger log)
        {
            log.LogInformation("***GetProductById HTTP trigger function processed a request.");

            if (productDetailsEntity == null)
            {
                log.LogInformation($"Product {id} not found");

                return new NotFoundResult();
            }

            return new OkObjectResult(productDetailsEntity.ToProductDetails());
        }
    }
}
