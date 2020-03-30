# Sassy Mail

### What is it?

Sassy mail is an email framework made easy using Sass. It is highly tested and made with cross client and browser compatibility in mind, with the goal of providing a seamless experience even in the more 'hardcore' outlook versions.

#### Features

* Pre-set button styles and mixins that you can easily customise
* Logical Sass mixins and file organization
* Control layout, typography, colour schemes, etc via a variables Sass file
* Comes with premailer set up to easily inline code
* Mobile responsive configurable grid which automatically calculates widths


### Usage

#### Compass
To get started, you'll need Sass and Compass.

Install compass via gem:
```
$ gem update --system
$ gem install compass
```

If you need more information about compass installation, please refer to its [documentation](http://compass-style.org/install/).

#### Package installation

To install all dependencies and NPM is recommended. 

After cloning the repository:

```
$ cd sass
$ npm install
```

This will get you up and running with grunt and the following usage commands.

#### Grunt commands

`grunt no-comments`
Removes comments from generated CSS files

`grunt email`
Removes unnecesary CSS and adds it to head of new tidy.html file.

`grunt inline`
Removes unnecesary CSS and adds it to head of new tidy.html file and creates new email-inline.html file

`grunt full`
Removes comments from generated CSS files, removes unnecesary CSS and adds it to head of new tidy.html file and creates new email-inline.html file

[Premailer](https://github.com/premailer/premailer/) will be used for inlining