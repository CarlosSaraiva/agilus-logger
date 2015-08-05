var Service = require("node-windows").Service;
var wincmd = require("node-windows");
var argv = require("minimist")(process.argv.slice(2));

//Variaveis do serviço
var port = argv.p,
    name = argv.n,
    appName = "Agilus Logger - " + name + " (porta: " + port + " )";

// Create a new service object
var svc = new Service({
    name: "Agilus Logger - " + name + " (porta: " + port + " )",
    description: 'Teste',
    script: require('path').join(__dirname, 'agilus-logger.js'),
    env: [
        {
            name: 'NODE_ENV',
            value: 'production'
        },
        {
            name: 'port',
            value: port
        },
        {
            name: 'appName',
            value: 'Agilus Logger - ' + name
        }
    ]    
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
    console.log(svc.name + " iniciado...\nServiço disponível na porta: " + argv.p + " em: " + argv.n);
});

// Install the script as a service.
svc.install();