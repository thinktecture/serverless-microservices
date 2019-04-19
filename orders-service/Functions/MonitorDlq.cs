using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace Serverless
{
    public class MonitorDlqFunctions
    {
        [FunctionName("MonitorShippingsInitiatedDlq")]
        public void MonitorShippingsInitiatedDlq(
            [ServiceBusTrigger("shippingsinitiated/$DeadLetterQueue", Connection = "ServiceBus")]
            string queueMessage,
            [SendGrid(ApiKey = "SendGrid")]
            out SendGridMessage email,
            ILogger log)
        {
            log.LogInformation($"MonitorShippingsInitiatedDlq ServiceBus queue trigger function processed message: {queueMessage}");

            // TODO: implement logic that inspects message and decides what to do with it!!!

            email = SendEmail("ShippingsInitiated", queueMessage);
        }

        [FunctionName("MonitorOrdersForShippingDlq")]
        public void MonitorOrdersForShippingDlq(
            [ServiceBusTrigger("ordersforshipping/$DeadLetterQueue", Connection = "ServiceBus")]
            string queueMessage,
            [SendGrid(ApiKey = "SendGrid")]
            out SendGridMessage email,
            ILogger log)
        {
            log.LogInformation($"MonitorOrdersForShippingDlq ServiceBus queue trigger function processed message: {queueMessage}");

            // TODO: implement logic that inspects message and decides what to do with it!!!

            email = SendEmail("OrdersForShipping", queueMessage);
        }

        [FunctionName("MonitorNewOrdersDlq")]
        public static void MonitorNewOrdersDlq(
            [ServiceBusTrigger("neworders/$DeadLetterQueue", Connection = "ServiceBus")]
            string queueMessage,
            [SendGrid(ApiKey = "SendGrid")]
            out SendGridMessage email,
            ILogger log)
        {
            log.LogInformation($"MonitorNewOrdersDlq ServiceBus queue trigger function processed message: {queueMessage}");

            // TODO: implement logic that inspects message and decides what to do with it!!!

            email = SendEmail("NewOrders", queueMessage);
        }

        private static SendGridMessage SendEmail(string serviceName, string queueMessage)
        {
            SendGridMessage email = new SendGridMessage();
            email.AddTo("christian.weyer@thinktecture.com");
            email.AddContent("text/html", "PROBLEM: <br><pre>" + queueMessage + "</pre>");
            email.SetFrom(new EmailAddress("azuremonitor@thinktecture.com"));
            email.SetSubject(String.Format($"Azure Serverless Microservices: Problem with {0}!", serviceName));

            return email;
        }
    }
}
