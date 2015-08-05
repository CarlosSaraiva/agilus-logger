var gulp = require("gulp"),
    concat = require("gulp-concat"),
    uglify = require("gulp-uglify"),
    del = require("del")

var config = {
    src: ["app/**/*.js", "!app/**/*.min.js"]
}

gulp.task("clean", function() {
    del.sync([""]);
});

gulp.task("scripts", ["clean"], function() {
    return gulp.src(config.src)
        .pipe(uglify())
        .pipe(concat("all.min.js"))
        .pipe(gulp.dest("app/"));
});

gulp.task("default", ["scripts"], function () { });

