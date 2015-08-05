/// <vs AfterBuild='copy' />
var gulp = require("gulp"),
    copy = require("gulp-copy");

gulp.task("copy", function() {
    gulp.src("agilus-logger.js")
        .pipe(gulp.dest("./build"));
});