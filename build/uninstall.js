var Service = require("node-windows").Service,
    xml = require("xml2js"),
    fs = require("fs");

fs.readdir("/daemon", function(list, err) {
    if (!err) {        
        console.log(list.match(/\w+\D?\.xml/g));

    }

});

var execName = fs.readdir("daemon/", function(files, err) {
    if (!err) {
        console.log(files);
    } else {
        throw (err);
    }
});

// Create a new service object
var svc = new Service({
    name: "Agilus Logger",
    script: require("path").join(__dirname, "agilus-logger.js")
});

// Listen for the "uninstall" event so we know when it's done.
svc.on("uninstall", function () {
    console.log("Serviço desinstalado");
    console.log("Serviço no sistema? ", svc.exists);
});

// Uninstall the service.
svc.uninstall();