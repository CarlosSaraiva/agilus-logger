var Service = require("node-windows").Service;
var wincmd = require("node-windows");
var argv = require("minimist")(process.argv.slice(2));

// Create a new service object
var svc = new Service({
    name: argv.e + " Logger",
    description: "",
    script: require("path").join(__dirname, "agilus-logger.js"),
    env: [
        {
            name: "NODE_ENV",
            value: "production"
        },
        {
            name: "p",
            value: argv.p
        }
    ],
    exe:  "logger-" + argv.e
});

// Listen for the "install" event, which indicates the
// process is available as a service.
svc.on("install", function () {
    svc.start();
});

// Just in case this file is run twice.
svc.on("alreadyinstalled", function () {
    console.error("Este serviço já esta instalado no sistema.");
});

// Listen for the "start" event and let us know when the
// process has actually started working.
svc.on("start", function () {
    console.log(svc.name + " iniciado...\nServiço disponível na porta: " + argv.p + " em: " + argv.e);
});

// Install the script as a service.
svc.install();