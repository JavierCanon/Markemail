var gulp = require('gulp');
var autoprefixer = require('autoprefixer');
var cssnano = require('cssnano');
var precss = require('precss');
var stylelint = require('stylelint');
var uglify = require('gulp-uglify');
var pump = require('pump');
var postcss = require('gulp-postcss');
var concat = require('gulp-concat');




gulp.task('build', ['compress', 'css']);

gulp.task('css',function(){
	var processors = [
		autoprefixer,
		cssnano				
	];
	return gulp.src('web/bundles/css/all.css')
		.pipe(postcss(processors))
		.pipe(concat('all.min.css'))
		.pipe(gulp.dest('./web/bundles/css/'));
});
gulp.task('compress', function() {
  return gulp.src(
	[
	'web/bundles/app/js/01_croppie.js',
	'web/bundles/app/js/02_jquery.tablesorter.min.js',
	'web/bundles/app/js/03_builderscripts.js',
	'web/bundles/app/js/imagecropandupload.js',
	'web/bundles/app/js/scripts.js'
	]) //return gulp.src('./app/js/*.js')
		.pipe(uglify())
		.pipe(concat('all.min.js'))
    .pipe(gulp.dest('./web/bundles/app/js'));
});

gulp.task('compress-old', function (cb) {
  pump([
        gulp.src('app/**/all.js'),//gulp.src('app/**/*.js'),
        uglify(),
        gulp.dest('./dist')
    ],
    cb
  );
});

gulp.task('prettify', function() {
  gulp.src('app/**/*.js')
    .pipe(prettify({
			collapseWhitespace: true
		}))
    .pipe(gulp.dest('./src')) // edit in place 
});
gulp.task('prettify2', function() {
  gulp.src(['app/**/*.js', 'app/**/*.css'])
    .pipe(prettify())
    .pipe(gulp.dest('./src')) // edit in place 
});
//gulp.watch('app/**/*.css', ['css']); 
gulp.task('watch', function(){
  gulp.watch('app/**/*.css', ['css']); 
	gulp.watch('app/**/*.js', ['compress']); 

});
