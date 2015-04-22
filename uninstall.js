var Service = require('node-windows').Service;

// Create a new service object
var svc = new Service({
  name:'Logger',
  script: require('path').join(__dirname,'router.js')
});

// Listen for the "uninstall" event so we know when it's done.
svc.on('uninstall',function(){
  console.log('Serviço desinstalado');
  console.log('Serviço no sistema? ', svc.exists);
});

// Uninstall the service.
svc.uninstall();
