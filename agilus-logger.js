//Importando módulos
var http = require("http");
var Router = require("node-simple-router");
var sql = require("mssql");
var fs = require("fs");
var path = require("path");
var EventLogger = require("node-windows").EventLogger;
var minimist = require("minimist");

//Instanciando objetos e declarando/iniciando variáveis
var router = new Router();
var server = http.createServer(router);
var log = new EventLogger("Agilus Logger");
var tabela = "insert ligacao_telefonica(lit_nome_empresa, lit_data, lit_origem, lit_destino, lit_duracao_total, lit_duracao_conversacao, lit_status, lit_identificador_gravacao, lit_codigo_agilus)";
var connectionString, connectionErrorFlag;

//Argumentos do prompt
console.log(minimist);

//Leitura do arquivo de configuração e Inicio do server
readUDL("agilus.udl", function () {
    server.listen(1330, "0.0.0.0", function () {
        log.info("Server iniciado na porta 1330");
    });
});

//Caminho para a chave de verificação do Loader.io
router.get("/loaderio-f0d7632af1978f16170fae890d111b60", function (request, response) {
    "use strict";
    response.end("loaderio-f0d7632af1978f16170fae890d111b60");
});

//Rotas
router.post("/insert", function (request, response) {
    var query = insertString(request);
    database(query, function (result) {
        response.end(JSON.stringify(result));
    });
});

//Funções auxiliares
//Leitura do arquivo de configuração
function readUDL(file, callback) {
    var f = fs.readFile(path.join(__dirname, file), "ucs2", function (fileError, data) {
        if (fileError) {
            console.log("Arquivo de configuração não encontrado ou corrompido. Server não pode ser iniciado.");
            log.error("Arquivo de configuração não encontrado ou corrompido. Server não pode ser iniciado.", 1000, function () {
                process.exit(1);
            });
        } else {
            var udl = JSON.parse(UDLtoJSON(data));
            connectionString = {
                user: udl.UserID !== undefined ? udl.UserID : "",
                password: udl.Password !== undefined ? udl.Password : "",
                server: udl.DataSource !== undefined ? udl.DataSource : "",
                database: udl.InitialCatalog !== undefined ? udl.InitialCatalog : "",
                appName: "Agilus Logger"
            };
            callback();
        }
    });
}

//Monta a string do insert que sera executado no banco de dados
function insertString(request) {
    var values = "'" + request.body.nome + "'," +
        "'" + request.body.calldate + "'," +
        "'" + request.body.src + "'," +
        "'" + request.body.dst + "'," +
        "'" + request.body.duration + "'," +
        "'" + request.body.billsec + "'," +
        "'" + request.body.disposition + "'," +
        "'" + request.body.userfield + "'," +
        "'" + request.body.callid + "'";
    return tabela + "values(" + values + ")";
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
                    log.error(queryError.name + ": " + queryError.message, 1000);
                    callback(queryError);
                }
                connection.close();
            });
        } else {
            log.error(connectionError.name + ": " + connectionError.message, 1000);
            callback(connectionError);
        }
    });
}

//Função que converte arquivos udl para o formato json
function UDLtoJSON(data) {
    var udl = {};

    data.split(";").map(function(e) {
        var param = e.replace(/[|]|\n|\r| | [oledb]/g, "").split("=");
        udl[param[0]] = param.length === 2 ? param[1] : "";
    });

    return JSON.stringify(udl);
}