import { AzureFunction, Context } from "@azure/functions"

const timerTrigger: AzureFunction = async function (context: Context, timer: any): Promise<void> {
    var timeStamp = new Date().toISOString();
    context.log('TS timer trigger function ran.', timeStamp);   
};

export default timerTrigger;
