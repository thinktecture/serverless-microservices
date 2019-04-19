using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Serverless
{
    public class ProductDetails
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool OnStock { get; set; }
    }
}