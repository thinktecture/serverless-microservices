using System;
using MessagePack;
using Microsoft.WindowsAzure.Storage.Table;

namespace Serverless
{
    [MessagePackObject]
    public class ProductDetails
    {
        [Key(0)]
        public Guid Id { get; set; }
        [Key(1)]
        public string Name { get; set; }
        [Key(2)]
        public string Description { get; set; }
        [Key(3)]
        public bool OnStock { get; set; }
    }
}