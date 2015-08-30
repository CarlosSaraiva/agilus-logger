var Service = require("node-windows").Service,
    xml = require("xml2js"),
    fs = require("fs"),
    argv = require("minimist")(process.argv.slice(2));

var serviceName,
    svc;

svc = new Service({
    name: argv.s.replace(/.exe/, ""),
    script: require("path").join(__dirname, argv.n + "/agilus-logger.js")
});

svc.on("uninstall", function () {
    console.log("Serviço desinstalado");
    console.log("Serviço no sistema? ", svc.exists);
});

svc.uninstall();