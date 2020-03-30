# Contributing guidelines
```Last Updated: 2018-08-16 by  Javier Cañon```

## Pull Request Checklist

Before sending your pull requests, make sure you followed this list.

- Read [contributing guidelines](CONTRIBUTING.md).
- Read [Code of Conduct](CODE_OF_CONDUCT.md).
- Check if my changes are consistent with the guidelines.
- Changes are consistent with the Coding Style.
- Run Unit Tests.

## How to become a contributor and submit your own code

### Contributor License Agreements

We'd love to accept your patches! Before we can take them, we have to jump a couple of legal hurdles.
Please read the License Agreement.
***NOTE***: Only original source code from you and other people can be accepted into the main repository.

### Contributing code

If you have improvements to TensorFlow, send us your pull requests! For those
just getting started, Github has a [howto](https://help.github.com/articles/using-pull-requests/).

TensorFlow team members will be assigned to review your pull requests. Once the pull requests are approved and pass continuous integration checks, we will merge the pull requests.
For some pull requests, we will apply the patch for each pull request to our internal version control system first, and export the change out as a new commit later, at which point the original pull request will be closed. The commits in the pull request will be squashed into a single commit with the pull request creator as the author. These pull requests will be labeled as pending merge internally.

If you want to contribute but you're not sure where to start, 
take a look at the issues section.
 
These are issues that we believe are particularly well suited for outside
contributions, often because we probably won't get to them right now. If you
decide to start on an issue, leave a comment so that other people know that
you're working on it. If you want to help out, but not alone, use the issue
comment thread to coordinate.

### Contribution guidelines and standards

Before sending your pull request for review
make sure your changes are consistent with the guidelines and follow the coding style.

#### General guidelines and philosophy for contribution

* Include unit tests when you contribute new features, as they help to
  a) prove that your code works correctly, and b) guard against future breaking
  changes to lower the maintenance cost.
* Bug fixes also generally require unit tests, because the presence of bugs
  usually indicates insufficient test coverage.
* Keep compatibility in mind when you change code in core; 
  e.g., code has reached version 1 and hence cannot make
  non-backward-compatible changes without a major release. Reviewers of your
  pull request will comment on any compatibility issues.
* When you contribute a new feature, the maintenance burden is (by
  default) transferred to the team. This means that benefit of the
  contribution must be compared against the cost of maintaining the feature.
* Full new features (e.g., a new op implementing a cutting-edge algorithm)
  typically will live in a branch,to get some airtime before decision 
  is made regarding whether they are to be migrated to the core.

#### License

Include a (the) license at the top of new files you can use a developer tool,
e.g.: [License Header Manager](https://github.com/rubicon-oss/LicenseHeaderManager)  

#### Coding style languages

* [Microsoft C# and .NET Style Guide](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/)
* [Google Java Style Guide](https://google.github.io/styleguide/javaguide.html)
* [Google JavaScript Style Guide](https://google.github.io/styleguide/jsguide.html)
* [Google Shell Style Guide](https://google.github.io/styleguide/shell.xml)
* [Google Objective-C Style Guide](https://google.github.io/styleguide/objcguide.html)
* [Google Python Style Guide](https://github.com/google/styleguide/blob/gh-pages/pyguide.md)
* [Google C++ Style Guide](https://google.github.io/styleguide/cppguide.html).


#### Running unit tests

There are many ways to run unit tests, like:
1. Using tools and libraries installed directly on your system.
2. Using [Docker](https://www.docker.com). [Microsoft Docker Help](https://docs.microsoft.com/en-us/dotnet/core/docker/).
3. [Visual Studio Unit Testing](https://docs.microsoft.com/en-us/dotnet/core/testing/) 


