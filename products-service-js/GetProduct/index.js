module.exports = function (context, req) {
    context.log('Start GetProduct...');

    var product = context.bindings.product;

    if(product) {
        var result = {
            id: product.RowKey, name: product.Name, description: product.Description, onStock: product.OnStock
        };
    
        context.res.status(200).json(result);    
    } else {
        context.res.status(404);
        context.done();
    }
};