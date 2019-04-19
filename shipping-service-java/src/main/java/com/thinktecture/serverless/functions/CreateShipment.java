package com.thinktecture.serverless.functions;

import java.io.IOException;
import java.util.Date;
import java.util.UUID;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.microsoft.azure.functions.ExecutionContext;
import com.microsoft.azure.functions.annotation.FunctionName;
import com.microsoft.azure.functions.annotation.ServiceBusQueueOutput;
import com.microsoft.azure.functions.annotation.ServiceBusQueueTrigger;
import com.thinktecture.serverless.messages.NewOrderMessage;
import com.thinktecture.serverless.messages.ShippingCreatedMessage;

/**
 * Azure Functions with Service Bus Trigger.
 */
public class CreateShipment {
    /**
     * This function will be invoked when a new message is received at the Service
     * Bus Queue.
     * 
     * @throws InterruptedException
     * @throws IOException
     */
    @FunctionName("CreateShipment")
    @ServiceBusQueueOutput(name = "$return",
        queueName = "shippingsinitiated",
        connection = "ServiceBus")
    public String /*ShippingCreatedMessage*/ run(
            @ServiceBusQueueTrigger(name = "message", 
                queueName = "ordersforshipping", 
                connection = "ServiceBus")
            NewOrderMessage msg,
            final ExecutionContext context
    ) throws InterruptedException, IOException {
        context.getLogger().info("CreateShipment ServiceBus queue trigger function processed message.");
        context.getLogger().info(msg.Order.Description);

        // NOTE: Look at our complex business logic! :-)
        // Yes - do the REAL STUFF here...
        Thread.sleep(5000);

        ShippingCreatedMessage shippingCreated = new ShippingCreatedMessage();
        shippingCreated.Id = UUID.randomUUID();
        shippingCreated.Created = new Date();
        shippingCreated.OrderId = msg.Order.Id;
        shippingCreated.UserId = msg.UserId;

        Gson gson = new GsonBuilder().setDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSSSSS'Z'").create();
        String shippingCreatedMessage = gson.toJson(shippingCreated);

        context.getLogger().info("ShippingCreated Message: " + shippingCreatedMessage);

        return shippingCreatedMessage;
    }
}
