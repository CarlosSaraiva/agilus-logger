var nexe = require("nexe");

nexe.compile({
    input: 'router.js',
    output: 'router',
    nodeVersion: '0.12.0',
    nodeTempDir: 'temp',
    python: 'C:/Python27',
  
    flags: true,
    framework: "nodejs"
}, function(err) {
    console.log(err);
});