# bovender
This is my inhouse C# framework that implements MVVM pattern and more.

I am an enthusiast/hobby/freelance programmer. I have created this framework
because other, professional approaches such as the [Prism][] guidance and
framework were too heavy for my purposes. Of course I realize that most
people won't be interested in this framework (because they would either
use one of the professional large frameworks or create their own
home-grown one), but since I produce open-source software, I have to put
the source code somewhere...


## NuGet package

This framework is available as a [NuGet][] package to facilitate its use in
multiple projects.


## Required references

To build a project that uses the Bovender framework, add the following
refrences:

- PresentationCore
- PresentationFramework
- System.Xaml
- WindowsBase


## Documentation

[Doxygen][] documentation can be found at the [GitHub page][gh-pages] of
this project.


## Versioning and changelog

This framework is [semantically versioned][semver].

Please inspect the git log for changes.


## High-level overview

Here is a high-level overview of the namespaces in this framework.
Please see the [docs][gh-pages] for detailed information.


### Bovender.Mvvm


### Bovender.ExceptionHandler


### Bovender.Unmanaged


### Bovender.Versioning

The `Versioning` namespace provides a class `SemanticVersion` to
facilitate handling [semantic versions][semver].


### Bovender.HtmlFiles


### Bovender.Text


### Bovender

A few classes live in the main namespace of Bovender:

- `FileHelpers`: Helper methods to deal with files (currently only a
  static method `Bovender.FileHelpers.Sha1Hash()` that returns the Sha1
  checksum of a file as string).
- `PathHelpers`: Improvements (in my eyes) on some of the static methods
  provided by `System.IO.Path`:
  `Bovender.PathHelpers.GetDirectoryPart()` extracts the directory part
  of a path (where the path may end with a file name or a directory
  name), and `Bovender.PathHelpers.GetFileNamePart()` extracts the file
  name (if present) of a path.


### License

This framework is distributed under the [Apache License 2.0][apache].

[Prism]: https://msdn.microsoft.com/en-US/library/ff648465.aspx
[NuGet]: https://www.nuget.org/packages/Bovender
[gh-pages]: http://bovender.github.io/bovender
[apache]: http://www.apache.org/licenses/LICENSE-2.0
[semver]: http://semver.org
[Doxygen]: http://www.doxygen.org

<!-- vim: set tw=72 ai fo+=tqn wrap : -->
