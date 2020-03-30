let argv = require('yargs').argv;
let bin = require('./bin');
let command = require('node-cmd');
let execSync = require('child_process').execSync;

let AfterWebpack = require('on-build-webpack');
let AfterJigsaw = require('./after-jigsaw');
let BrowserSync = require('browser-sync');
let BrowserSyncPlugin = require('browser-sync-webpack-plugin');
let Watch = require('webpack-watch');

let env = argv.e || argv.env || 'local';
let port = argv.p || argv.port || 3000;

let browserSyncInstance;
let config = JSON.parse(execSync('php ./tasks/php/config -e' + env));

if (!config) {
  console.log('\nBuild aborted: Jigsaw config is '+ typeof config +'.\n');
  process.exit();
}

let build_path = config.build && config.build.destination ? config.build.destination : 'build_' + env;
let prettyURLs = config.pretty == false ? '--pretty=false ' : '';

module.exports = {
  jigsaw: new AfterWebpack(() => {

    command.get(bin.path() + ' build ' + prettyURLs + env, (error, stdout, stderr) => {
      console.log(error ? stderr : stdout);

      AfterJigsaw.processEmails(config);

      if (browserSyncInstance) {
        browserSyncInstance.reload();
      }
    });

  }),

  watch: (paths) => {
    return new Watch({
      options: { ignoreInitial: true },
      paths: paths,
    })
  },

  browserSync: (proxy) => {
    return new BrowserSyncPlugin({
      notify: false,
      port: port,
      proxy: proxy,
      tunnel: config.browsersync.tunnel || false,
      server: proxy ? null : { baseDir: build_path, directory: config.browsersync.listing || true },
    },
    {
      reload: false,
      callback: () => {
        browserSyncInstance = BrowserSync.get('bs-webpack-plugin');
      },
    })
  },
};
