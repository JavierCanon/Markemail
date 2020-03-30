module.exports = function(grunt) {

  // Project configuration.
  grunt.initConfig({
    pkg: grunt.file.readJSON('package.json'),
    uncss: {
		dist: {
			src: ['templates/email.html'],
			dest: 'stylesheets/tidy.css',
			options: {
				report: 'min', // optional: include to report savings
				ignore: [/^\.ExternalClass.*/, 'div[style*="margin: 16px 0"]', '.yshortcuts a'] // Client specific fixes and classes
			}
		}
	},
	processhtml: {
	  dist: {
		files: {
		  'templates/tidy-email.html': ['templates/email.html']
		}
	  }
	},
	stripCssComments: {
        dist: {
            files: {
                'stylesheets/email.css': 'stylesheets/email.css'
            }
        }
    },
	premailer: {
	  main: {
		options: {
		  verbose: true,
		  preserveStyles: true,
		  removeComments:false,
		  warnLevel:'risky'
		},
		files: {
		  'templates/email-inline.html': ['templates/tidy-email.html']
		}
	  }
	}
 });

  // Load the plugins 
 grunt.loadNpmTasks('grunt-uncss');
 grunt.loadNpmTasks('grunt-processhtml');
 grunt.loadNpmTasks('grunt-premailer');
 grunt.loadNpmTasks('grunt-strip-css-comments');

  // task(s).
 grunt.registerTask('full', ['stripCssComments' ,'uncss', 'processhtml', 'premailer']);
 grunt.registerTask('inline', ['uncss', 'processhtml', 'premailer']);
 grunt.registerTask('email', ['uncss', 'processhtml']);
 grunt.registerTask('no-comments', ['stripCssComments']);
};