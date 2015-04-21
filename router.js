//Importando módulos
var http = require('http');
var Router = require('node-simple-router');
var sql = require("mssql");
var fs = require("fs");
var path = require('path');
var EventLogger = require('node-windows').EventLogger;

//Instanciando objetos
var router = new Router();
var server = http.createServer(router);
var log = new EventLogger('Test');

//Leitura arquivo de configuração
var file = fs.readFileSync(path.join(__dirname, 'sql.udl'), 'ucs2').split(';');
var connectionString = {
    user: file[4].split('=')[1],
    password: file[2].split('=')[1],
    server: file[6].split('=')[1].replace('\n', '').replace('\r', ''),
    database: file[5].split('=')[1],
    appName: 'test'
};

//Inicio server
server.listen(1330, function () {
    log.info('Server started listening at port 1330');
    console.log('Server listening at 1330');
});

//Rotes
router.post("/insert", function(request, response) {
    var query = queryString(request);
    database(query, function(result) {
        console.log(result);
        response.end(JSON.stringify(result));
    });
});

//Funções auxiliares
function queryString(request) {
    var param = [];
    for (var item in request.post) {
        if (request.post[item] !== undefined) param.push("'" + request.post[item] + "'");
        else param.push();
    }
    return "insert into ligacao_telefonica values(" + param + ")";
}

function database(query, callback) {
    var connection = new sql.Connection(connectionString, function(err) {
        if (!err) {
            var request = new sql.Request(connection);
            request.query(query, function(err) {
                if (!err) {
                    callback('Done');
                } else {
                    log.error(err);
                    callback(err);
                }
            });
        } else {
            log.error(err);
            callback(err);
        }
    });
}