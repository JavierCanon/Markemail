![MailGrid](http://rapidwebspace.com/MailGrid/logo-dark.png)
==========

Our new responsive email workflow that is a fork from [Daryll Doyle's](https://github.com/darylldoyle/Gulp-Email-Creator) great work!

# Installation

To use MailGrid you need to have both [Node.js](http://nodejs.org/) and [Gulp.js](http://gulpjs.com/) installed . 

You can install it by cloning this repository to a local folder and running the following from inside the directory.

```javascript
    npm install
```
This will install all dependencies.

You will need to get a Mailchimp API by creating one in your Mailchimp account. [Instructions Here](http://kb.mailchimp.com/accounts/management/about-api-keys).

# Using the package

You can use the MailGrid by creating your HTML in the `./src/html/` directory and your LESS in the `./src/less` directory.

## Compile Templates
Running `gulp` from the terminal will build the new HTML email template into the `./output/` directory and the proceed to watch `./src/html/` and `./src/less/` for any updates.

The gulpfile has browsersync built in and will reload on any HTML or CSS edit, allowing you to focus on your code.

## Sending the Template to yourself
Running `gulp send` will allow you to send the compiled template to yourself through [Mailgun](https://mailgun.com) (you'll need to sign up for a free account). The settings for this can be found in the `config.json` file.

To run this, you can use the following command

```javascript
gulp send --template="compiled-template-name.html"
```

##Testing

You can now throw your emails straight into [Litmus](http://litmus.com/) tests. You'll need an active [Litmus](http://litmus.com/) account and to get your [static email](https://litmus.com/static-email). Plug this into the `config.json` file and then run the following command.

```javascript
gulp litmus --template="compiled-template-name.html"
```
In a few seconds/minutes, you'll see the test appear in Litmus for you!

# Contributing

Any contributions will be happily received. Simply open a [new issue](https://github.com/rapidwebltd/MailGrid/issues) or create a new [pull request](https://github.com/rapidwebltd/MailGrid/pulls). However, please consider if your contributions would be more suitable upstream. 
