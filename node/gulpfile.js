/// <vs BeforeBuild='flat' AfterBuild='build' Clean='clean-packages' />
var gulp = require("gulp"),    
    exec = require("child_process").exec;

var srcPath = "./src/";

gulp.task("flat", function(cb) {
    exec("flatten-packages", function(err, stdout, stderr) {
        console.log(stdout);
        console.log(stderr);
        cb(err);
    });
});

gulp.task("build", function() {
    gulp.src([srcPath + "agilus-logger.js", srcPath + "install.js", srcPath +"uninstall.js"])        
        .pipe(gulp.dest("./build"));
});