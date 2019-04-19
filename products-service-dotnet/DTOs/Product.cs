using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace Serverless
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}