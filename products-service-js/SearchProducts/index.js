var request = require('request');
var adminKey = process.env['SearchServiceKey'];

module.exports = function (context, req) {
    context.log('Start SearchProducts...');
    
    const searchterm = req.params.searchterm;

    if (searchterm) {
        var url = process.env['SearchServiceUrl'] + 
            searchterm;
        
        var headers = {'api-key': adminKey};

        var options = {
            url: url,
            headers: headers,
            withCredentials: false
        };
        
        request.get(options, function(error, response, body){
            var data = JSON.parse(body).value;

            if (!error) {
                var result = data.map(r => {
                    return { id: r.RowKey, name: r.Name, description: r.Description }
                });
                context.res.status(200).json(result);
            }
            else {
                context.res.status(500).json({error : error});
            }
        });
    }
    else {
        context.res.status(404);
        context.done();
    }
};