using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Serverless
{
    public class ProductEntity : TableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Product ToProduct()
        {
            return new Product
            {
                Id = Guid.Parse(this.RowKey),
                Name = this.Name
            };
        }
    }
}