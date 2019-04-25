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
    public class ProductsFunctions
    {
        private const string ALLPRODUCTS = "allproducts";

        [FunctionName("ListProducts")]
        public async Task<IActionResult> ListProducts(
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
                var productsFromCache = await FillProductsCache(productsTable, cache);
                products = productsFromCache.Select(p => new Product{ Id = p.Id, Name = p.Name }).ToList();
            }

            return new OkObjectResult(products);
        }

        [FunctionName("GetProduct")]
        public async Task<ActionResult> GetProduct(
            [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "products/{id}")]
            HttpRequest req,
            
            string id,
            
            [Table("products", "products")]
            CloudTable productsTable,

            [Inject]
            IMemoryCache cache,

            ILogger log)
        {
            log.LogInformation("***GetProduct HTTP trigger function processed a request.");

            List<ProductDetails> products;

            if(!cache.TryGetValue(ALLPRODUCTS, out products))
            {
                products = await FillProductsCache(productsTable, cache);
            }

            ProductDetails productDetails = products.Where(p => p.Id == Guid.Parse(id)).FirstOrDefault();

            if (productDetails == null)
            {
                log.LogInformation($"Product {id} not found");

                return new NotFoundResult();
            }

            return new OkObjectResult(productDetails);
        }

        private async Task<List<ProductDetails>> FillProductsCache(CloudTable productsTable, IMemoryCache cache)
        {
            List<ProductDetails> products;

            var query = new TableQuery<ProductDetailsEntity>();
            var productEntities = (await productsTable.ExecuteQuerySegmentedAsync(query, null)).ToList();
            
            products  = productEntities.Select(p => p.ToProductDetails()).ToList();

            var expirationTime = DateTime.Today.AddHours(1);
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(expirationTime);
            cache.Set(ALLPRODUCTS, products, cacheEntryOptions);

            return products;
        }
    }
}
