/*jslint node: true*/
var http = require('http');
var Router = require('node-simple-router');
var sql = require("mssql");

var router = Router();
var connectionString = "";
var result = "";

//Configurando conexao SQL
var configSql = {
    user: 'Claudio',
    password: 'afc@55216',
    server: 'servagilus.dyndns.org',
    database: 'dbDemo'
};

//Query


//routes
router.get("/hello", function(request, response) {
    response.end("Hello");
});

router.post("/search", function(request, response) {
    var query = "select * from ligacao_telefonica"; //where clt_nome like " + "'" + request.post.nome + "'";
    var connection = new sql.Connection(configSql, function(err) {
        var request = new sql.Request(connection);
        request.query(query, function(err, recordset) {
            response.end(JSON.stringify(recordset));
        });
    });
});

router.post("/insert", function(request, response) {
    var query = "insert into ligacao_telefonica values(" +
        "'" + request.post.nome + "'," +
        "'" + request.post.calldate + "'," +
        request.post.src + "," +
        request.post.dst + "," +
        request.post.duration + "," +
        request.post.billsec + "," +
        "'" + request.post.disposition + "', " +
        "'" + request.post.userfield + "'," +
        "'" + request.post.callid + "')";

    database(query, function(result) {
        console.log(result);
        response.end(JSON.stringify(result));
    });
});

function database(query, callback) {
    var connection = new sql.Connection(configSql, function(err) {
        var transaction = new sql.Transaction(connection);

        transaction.begin(function(err) {
            var request = new sql.Request(transaction);
            request.query(query).then(function(result) {
                transaction.commit();
                callback('Done');
            }).catch(function(err) {
                callback(err);
            });

        });
    });
}

//Inicializando server
var server = http.createServer(router);
server.listen(1330, function() {
    console.log("The service it's on at port 1330");
});