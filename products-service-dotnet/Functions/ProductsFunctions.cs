using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Linq;
using Willezone.Azure.WebJobs.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Distributed;
using MessagePack;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using System.Net;

namespace Serverless
{
    [StorageAccount("products")]
    public class ProductsFunctions
    {
        private const string ALLPRODUCTS = "allproducts";

        [FunctionName("ListProducts")]
        [OpenApiOperation("products", "Serverless Microservices")]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(List<Product>))]
        public async Task<IActionResult> ListProducts(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "api/products")]
            HttpRequest req,

            [Table("products", "products")]
            CloudTable productsTable,
            
            [Inject]
            IDistributedCache cache,
            
            ILogger log)
        {
            log.LogInformation("***ListProducts HTTP trigger function processed a request.");

            var productsFromCache = await GetProductsFromCache(productsTable, cache);
            var products = productsFromCache.Select(p => new Product { Id = p.Id, Name = p.Name }).ToList();

            return new OkObjectResult(products);
        }

        [FunctionName("GetProduct")]
        [OpenApiOperation("productbyid", "Serverless Microservices")]
        [OpenApiResponseBody(HttpStatusCode.OK, "application/json", typeof(ProductDetails))]
        public async Task<ActionResult> GetProduct(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "api/products/{id}")]
            HttpRequest req,
            
            string id,
            
            [Table("products", "products")]
            CloudTable productsTable,

            [Inject]
            IDistributedCache cache,

            ILogger log)
        {
            log.LogInformation("***GetProduct HTTP trigger function processed a request.");

            var productsFromCache = await GetProductsFromCache(productsTable, cache);
            var productDetails = productsFromCache.FirstOrDefault(p => p.Id == Guid.Parse(id));

            if (productDetails == null)
            {
                log.LogInformation($"Product {id} not found");

                return new NotFoundResult();
            }

            return new OkObjectResult(productDetails);
        }

        private async Task<List<ProductDetails>> GetProductsFromCache(CloudTable productsTable, IDistributedCache cache)
        {
            List<ProductDetails> productsFromCache;

            var productsRaw = await cache.GetAsync(ALLPRODUCTS);

            if (productsRaw != null)
            {
                productsFromCache = MessagePackSerializer.Deserialize<List<ProductDetails>>(productsRaw);
            }
            else
            {
                productsFromCache = await FillProductsCache(productsTable, cache);
            }

            return productsFromCache;
        }

        private async Task<List<ProductDetails>> FillProductsCache(CloudTable productsTable, IDistributedCache cache)
        {
            var query = new TableQuery<ProductDetailsEntity>();
            var productEntities = (await productsTable.ExecuteQuerySegmentedAsync(query, null)).ToList();
            
            var products  = productEntities.Select(p => p.ToProductDetails()).ToList();

            var expirationTime = DateTime.Today.AddHours(1);
            var cacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(expirationTime);
            await cache.SetAsync(ALLPRODUCTS, MessagePackSerializer.Serialize(products), cacheEntryOptions);

            return products;
        }
    }
}
