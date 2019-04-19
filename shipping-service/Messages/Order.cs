using System;
using System.Collections.Generic;

namespace Serverless
{
    public class Order
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public List<OrderItem> Items { get; set; }
    }
}