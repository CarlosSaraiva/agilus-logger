/// <vs AfterBuild='build' />
var gulp = require("gulp"),
    replace = require("gulp-replace-path");

var srcPath = "./src/";

gulp.task("build", function() {
    gulp.src([srcPath + "agilus-logger.js", srcPath + "install.js", srcPath +"uninstall.js", srcPath + "package.json"])
        .pipe(replace(/..\/node_modules\//g, ""))
        .pipe(gulp.dest("./build"));
});


