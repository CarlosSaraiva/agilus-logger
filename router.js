//Importando módulos
var http = require("http");
var Router = require("node-simple-router");
var sql = require("mssql");
var fs = require("fs");
var path = require("path");
var color = require("colors");

//Instanciando objetos
var router = new Router();
var server = http.createServer(router);
var litTabelaString = "insert ligacao_telefonica(lit_nome_empresa, lit_data, lit_origem, lit_destino, lit_duracao_total, lit_duracao_conversacao, lit_status, lit_identificador_gravacao, lit_codigo_agilus)";
var connectionString;

//Leitura do arquivo de configuração e Inicio do server
var file = fs.readFile(path.join(__dirname, "sql.udl"), "ucs2", function(fileError, data) {
    if (fileError) {
        console.log("Problemas com arquivo de configuração. Server não iniciado.".blue);
        process.exit();
    } else {
        var parameters = data.split(";");
        connectionString = {
            user: parameters[4].split("=")[1],
            password: parameters[2].split("=")[1],
            server: parameters[6].split("=")[1].replace("\n", "").replace("\r", ""),
            database: parameters[5].split("=")[1],
            appName: "Logger"
        };

        //Inicio do server
        server.listen(1330, function() {
            console.log('Servidor disponivel na porta:' + ' 1330'.green);
        });
    }
});

//Rotes
router.post("/insert", function(request, response) {
    var query = queryString(request);
    database(query, function(result) {
        response.end(result);
    });
});

//Funções auxiliares
function queryString(request) {
    var param = [];
    for (var item in request.post) {
        if (request.post[item] !== undefined) param.push("'" + request.post[item] + "'");
        else param.push();
    }
    return litTabelaString + "values(" + param + ")";
}

function database(query, callback) {
    var connection = new sql.Connection(connectionString, function(connectionError) {
        if (!connectionError) {
            var request = new sql.Request(connection);
            request.query(query, function(queryError) {
                if (!queryError) {
                    callback("Ok");
                } else {
                    console.error(JSON.stringify(queryError));
                    callback(JSON.stringify(queryError));
                }
                connection.close();
            });
        } else {
            console.error(JSON.stringify(connectionError));
            callback(JSON.stringify(connectionError));
        }
    });
}