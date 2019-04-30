import { AzureFunction, Context } from "@azure/functions"
import { Guid } from "guid-typescript";
import { ShippingCreatedMessage } from "../Messages/shippingCreatedMessage";

const serviceBusQueueTrigger: AzureFunction = async function (context: Context, message: NewOrderMessage): Promise<void> {
    context.log.info('***CreateShipment ServiceBus queue trigger function processed message', message);

    // NOTE: Look at our complex business logic!
    // TODO: Yes - do the REAL STUFF here...
    await sleep(5000);

    var shippingCreated = new ShippingCreatedMessage();
    shippingCreated.id = Guid.create();
    shippingCreated.created = new Date().toISOString();
    shippingCreated.orderId = message.order.id;
    shippingCreated.userId = message.userId;

    context.log.info("***New shipment: {0}", shippingCreated);

    try {
        context.bindings.outQueue = shippingCreated;
        context.done();
    } catch (err) {
        // TODO: retry policy...
        context.log.error("Service Bus Error", err);
        throw err;
    }
};

function sleep(ms: number) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

export default serviceBusQueueTrigger;
