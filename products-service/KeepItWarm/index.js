module.exports = async function (context, timer) {
    var timeStamp = new Date().toISOString();
    
    if(timer.isPastDue)
    {
        context.log('JavaScript is running late!');
    }
    context.log('JavaScript timer trigger function ran!', timeStamp);   
};