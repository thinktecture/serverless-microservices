module.exports = async function (context, req) {
    context.log('JavaScript Ping trigger function processed a request.');

    context.res = {
        status: 200,
        body: "OK"
    };
};