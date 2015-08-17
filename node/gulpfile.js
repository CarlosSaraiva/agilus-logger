/// <vs BeforeBuild='flat' AfterBuild='build' Clean='clean-packages' />
var gulp = require("gulp"),
    replace = require("gulp-replace-path"),
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
    gulp.src(srcPath + "agilus-logger.js")        
        .pipe(gulp.dest("./build/service"));

    gulp.src([srcPath + "install.js", srcPath + "uninstall.js"])
        .pipe(replace("/..\/node_modules\//g", ""))
        .pipe(gulp.dest("./build/logger"));
});