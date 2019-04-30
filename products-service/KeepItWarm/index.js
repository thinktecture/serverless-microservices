module.exports = async function (context, timer) {
    var timeStamp = new Date().toISOString();

    context.log('JavaScript timer trigger function ran.', timeStamp);   
};