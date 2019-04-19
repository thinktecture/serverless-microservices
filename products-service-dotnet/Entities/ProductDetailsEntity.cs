using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Serverless
{
    public class ProductDetailsEntity : TableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool OnStock { get; set; }

        public ProductDetails ToProductDetails()
        {
            return new ProductDetails
            {
                Id = Guid.Parse(this.RowKey),
                Name = this.Name,
                Description = this.Description,
                OnStock = this.OnStock
            };
        }
    }
}