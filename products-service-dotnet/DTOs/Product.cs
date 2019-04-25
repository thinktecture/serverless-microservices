using System;
using MessagePack;
using Microsoft.WindowsAzure.Storage.Table;

namespace Serverless
{
    [MessagePackObject]
    public class Product
    {
        [Key(0)]
        public Guid Id { get; set; }
        [Key(1)]
        public string Name { get; set; }
    }
}