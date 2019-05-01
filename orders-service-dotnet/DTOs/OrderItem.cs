using System;

namespace Serverless.DTOs
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
    }
}