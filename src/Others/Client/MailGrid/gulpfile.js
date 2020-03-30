// Include gulp
var gulp = require('gulp'),
less = require('gulp-less'),
LessPluginCleanCSS = require('less-plugin-clean-css'),
autoprefixer = require('gulp-autoprefixer'),
inline = require('gulp-mc-inliner'),
inlinesource = require('gulp-inline-source'),
util = require('gulp-util'),
nodemailer = require('nodemailer'),
fs = require('fs'),
html_strip = require('htmlstrip-native'),
cleancss = new LessPluginCleanCSS({ advanced: true, keepSpecialComments: 0 }),
path = require('path'),
//Include config file
config = require('./config.json');

// Build our templates
gulp.task('build', function() {
    return gulp.src('src/html/*.html')
        .pipe(inlinesource())
        .pipe(inline(config.APIKEY, false))
        .pipe(gulp.dest('./output'));
});

/*Compile LESS*/
gulp.task('less', function() {
    return gulp.src('src/less/*.less')
        .pipe(less({
          paths: [ path.join(__dirname, 'less', 'includes') ],
          plugins: [cleancss]
        }))
        .pipe(gulp.dest('src/css'));
});

// Watch Files For Changes
gulp.task('watch', function() {
    gulp.watch('src/less/*.less', ['less']);
    gulp.watch('src/html/*.html', ['build']);
    gulp.watch('src/css/*.css', ['build']);
});

// Default Task
gulp.task('default', ['less', 'build', 'watch']);

// Add ability to send test emails
gulp.task('send', function () {
    return sendEmail(util.env.template, config.testing.to);
});

gulp.task('litmus', function () {
    return sendEmail(util.env.template, config.litmus);
});

function sendEmail(template, recipient) {
    try {

        var options = {
            include_script : false,
            include_style : false,
            compact_whitespace : true,
            include_attributes : { 'alt': true }
        };

        var templatePath = "./output/" + template;

        var transporter = nodemailer.createTransport({
            service: 'Mailgun',
            auth: {
                user: config.auth.mailgun.user,
                pass: config.auth.mailgun.pass
            }
        });

        var templateContent = fs.readFileSync(templatePath, encoding = "utf8");

        var mailOptions = {
            from: config.testing.from, // sender address
            to: recipient, // list of receivers
            subject: config.testing.subject + ' - ' + template, // Subject line
            html: templateContent, // html body
            text: html_strip.html_strip(templateContent, options)
        };

        transporter.sendMail(mailOptions, function(error, info){
            if(error){
                return util.log(error);
            }else{
                return util.log('Message sent: ' + info.response);
            }
        });

    } catch (e) {
        if(e.code == 'ENOENT') {
            util.log('There was an error. Check your template name to make sure it exists in ./output');
        } else if(e instanceof TypeError) {
            util.log('There was an error. Please check your config.json to make sure everything is spelled correctly');
        } else {
            util.log(e);
        }
    }
}