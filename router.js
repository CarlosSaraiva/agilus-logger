//Importando módulos
var http = require("http");
var Router = require("node-simple-router");
var sql = require("mssql");
var fs = require("fs");
var path = require("path");
var EventLogger = require("node-windows").EventLogger;

//Instanciando objetos
var router = new Router();
var server = http.createServer(router);
var log = new EventLogger('Logger');
var litTabelaString = "insert ligacao_telefonica(lit_nome_empresa, lit_data, lit_origem, lit_destino, lit_duracao_total, lit_duracao_conversacao, lit_status, lit_identificador_gravacao, lit_codigo_agilus)";
var connectionString;

//Leitura do arquivo de configuração e Inicio do server
var file = fs.readFile(path.join(__dirname, "sql.udl"), "ucs2", function (fileError, data) {
    if (fileError) {
        console.log("Arquivo de configuração não encontrado ou corrompido. Server não pode ser iniciado.");
        log.error("Arquivo de configuração não encontrado ou corrompido. Server não pode ser iniciado.", 1000, function () {
            process.exit(1);
        });
    } else {
        var udl = JSON.parse(UDLtoJSON(data));

        connectionString = {
            user: udl.UserID,
            password: udl.Password,
            server: udl.DataSource,
            database: udl.InitialCatalog,
            appName: "Logger"
        };
        //Inicio do server
        server.listen(1330, function () {
            log.info("Server iniciado na porta 1330");
        });
    }
});

//Rotes
router.post("/insert", function (request, response) {
    var query = queryString(request);
    database(query, function (result) {
        response.end(JSON.stringify(result));
    });
});

//Funções auxiliares
//Monta a string do insert que sera executado no banco de dados
function queryString(request) {
    var param = [];
    for (var item in request.post) {
        if (request.post[item] !== undefined) param.push("'" + request.post[item] + "'");
        else param.push();
    }
    return litTabelaString + "values(" + param + ")";
}

//Função responsavel por conectar no banco de dados e fazer a inserção
function database(query, callback) {
    var connection = new sql.Connection(connectionString, function (connectionError) {
        if (!connectionError) {
            var request = new sql.Request(connection);
            request.query(query, function (queryError) {
                if (!queryError) {
                    callback("Ok");
                } else {
                    console.log(queryError.name + ": " + queryError.message);
                    log.error(queryError.name + ": " + queryError.message, 1000);
                    callback(queryError);
                }
                connection.close();
            });
        } else {
            console.log(JSON.stringify(connectionError.name + ": " + connectionError.message));
            log.error(connectionError.name + ": " + connectionError.message, 1000);
            callback(connectionError);
        }
    });
}

//Função que converte arquivos udl para o formato json
function UDLtoJSON(data) {
    var data = data.split(";")
    var udl = '';
    for (item in data) {
        var prop = data[item].replace(/[|]|\n|\r| | [oledb]/g, '').split("=");
        udl += prop.length > 1 ? '"' + prop[0] + '"' + ': ' + '"' + prop[1] + '"' + ',' : "";
    }
    return "{" + udl.slice(0, udl.length - 1) + "}";
}