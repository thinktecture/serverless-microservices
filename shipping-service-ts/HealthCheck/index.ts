import { AzureFunction, Context, HttpRequest } from "@azure/functions"

const httpTrigger: AzureFunction = async function (context: Context, req: HttpRequest): Promise<void> {
    context.log('Ping HTTP trigger function processed a request.');
    
    context.res = {
        body: "OK"
    };
};

export default httpTrigger;
