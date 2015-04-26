var Service = require('node-windows').Service;
var wincmd = require('node-windows');


wincmd.isAdminUser(function (isAdmin) {
    if (isAdmin) {
        console.log('Usuário é Administrador!');
    } else {
        console.error('Usuário NÃO é Administrador!');
    }
});

// Create a new service object
var svc = new Service({
    name: 'Logger',
    description: '',
    script: require('path').join(__dirname, 'router.js'),
    env: {
        name: "NODE_ENV",
        value: "production"
    }
});

// svc.user.domain = 'mydomain.local';
// svc.user.account = 'Carlos';
// svc.user.password = 'sde9016207946';

// Listen for the "install" event, which indicates the
// process is available as a service.
svc.on('install', function () {
    svc.start();
});

// Just in case this file is run twice.
svc.on('alreadyinstalled', function () {
    console.error('Este serviço já esta instalado no sistema.');
});

// Listen for the "start" event and let us know when the
// process has actually started working.
svc.on('start', function () {
    console.log(svc.name + ' iniciado...\nServiço disponível na porta 1330.');
});

// Install the script as a service.
svc.install();