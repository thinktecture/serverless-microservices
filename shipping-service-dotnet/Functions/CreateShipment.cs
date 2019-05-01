using System;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Serverless
{
    public static class CreateShipmentFunction
    {
        [FunctionName("CreateShipment")]
        public static async Task Run(
            [ServiceBusTrigger("ordersforshipping", Connection = "ServiceBus")]
            NewOrderMessage message,

            [ServiceBus("shippingsinitiated", Connection = "ServiceBus")]
            IAsyncCollector<ShippingCreatedMessage> shippingCreatedMessages,

            ILogger log)
        {
            log.LogInformation($"CreateShipment ServiceBus topic trigger function processed message: {message}");

            // NOTE: Look at our complex business logic!
            // TODO: Yes - do the REAL STUFF here...
            await Task.Delay(5000);

            var shippingCreated = new ShippingCreatedMessage
            {
                Id = Guid.NewGuid(),
                Created = DateTime.UtcNow,
                OrderId = message.Order.Id,
                UserId = message.UserId
            };

            log.LogInformation("New shipment: {0}", shippingCreated);

            try
            {
                await shippingCreatedMessages.AddAsync(shippingCreated);
                await shippingCreatedMessages.FlushAsync();
            }
            catch (ServiceBusException sbx)
            {
                // TODO: retry policy...
                log.LogError(sbx, "Service Bus Error");

                throw;
            }
        }
    }
}
