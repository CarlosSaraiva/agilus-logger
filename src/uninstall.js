var Service = require("node-windows").Service,
    xml = require("xml2js"),
    fs = require("fs");

var serviceName;
var svc;

fs.readdir(process.cwd() + "/daemon", function(err, list) {
    if (!err) {
        var parser = new xml.Parser();
        var name = list.filter(function(e) {
            return e.match(/\w+\D?\.xml/);
        });
        fs.readFile(__dirname + "/daemon/" + name[0], function (err, data) {
            parser.parseString(data, function(err, result) {
                svc = new Service({
                    name: result.service.name[0],
                    script: require("path").join(__dirname, "agilus-logger.js")
                });
                svc.on("uninstall", function () {
                    console.log("Serviço desinstalado");
                    console.log("Serviço no sistema? ", svc.exists);
                });
                svc.uninstall();
            });
        });

    } else {
        console.log(err);
    }
});
