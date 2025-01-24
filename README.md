<h1 align="center">
  <br>
  .NET Library Package Templates
  <br>
</h1>

<h4 align="center">"dotnet new" templates for building NuGet-published multi-targeting libraries with all the bells and whistles</h4>

<div align="center">

[![](https://img.shields.io/github/actions/workflow/status/dennisdoomen/dotnet-package-templates/build.yml?branch=main)](https://github.com/dennisdoomen/dotnet-package-templates/actions?query=branch%3amain)
[![](https://img.shields.io/github/release/dennisdoomen/dotnet-package-templates.svg?label=latest%20release&color=007edf)](https://github.com/dennisdoomen/dotnet-package-templates/releases/latest)
[![](https://img.shields.io/nuget/dt/DotNetLibraryPackageTemplates.svg?label=downloads&color=007edf&logo=nuget)](https://www.nuget.org/packages/DotNetLibraryPackageTemplates)
![GitHub Repo stars](https://img.shields.io/github/stars/dennisdoomen/dotnet-package-templates?style=flat)
[![GitHub contributors](https://img.shields.io/github/contributors/dennisdoomen/dotnet-package-templates)](https://github.com/dennisdoomen/dotnet-package-templates/graphs/contributors)
[![GitHub last commit](https://img.shields.io/github/last-commit/dennisdoomen/dotnet-package-templates)](https://github.com/dennisdoomen/dotnet-package-templates)
[![GitHub commit activity](https://img.shields.io/github/commit-activity/m/dennisdoomen/dotnet-package-templates)](https://github.com/dennisdoomen/dotnet-package-templates/graphs/commit-activity)
[![open issues](https://img.shields.io/github/issues/dennisdoomen/dotnet-package-templates)](https://github.com/dennisdoomen/dotnet-package-templates/issues)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg?style=flat-square)](https://makeapullrequest.com)

<a href="#about">About</a> •
<a href="#download">Download</a> •
<a href="#how-to-use-it">How To Use</a> •
<a href="#building">Contributors</a> •
<a href="#contributors">Contributors</a> •
<a href="#versioning">Versioning</a> •
<a href="#credits">Credits</a> •
<a href="#related">Related</a> •
<a href="#license">License</a>

</div>

## About

### What's this?

A bunch of `dotnet new` templates to quickly get you started building high-quality libraries including everything you need to publish it on NuGet or make it available as open-source.

It includes:
* Multi-targeting to cover as many .NET frameworks as possible
* Code coverage using [Coverlet](https://github.com/coverlet-coverage/coverlet) and [Coveralls.io](https://coveralls.io/)
* Static code analysis using Roslyn analyzers [StyleCopAnalyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers), [Roslynator](https://github.com/dotnet/roslynator), [CSharpGuidelinesAnalyzer](https://github.com/bkoelman/CSharpGuidelinesAnalyzer) and [Meziantou](https://github.com/meziantou/Meziantou.Framework).
* Auto-formatting using `.editorconfig` and settings honored by [JetBrains Rider](https://www.jetbrains.com/rider/) and [ReSharper](https://www.jetbrains.com/resharper/)
* A [Nuke](https://nuke.build/) C# build script that you can run locally as well as in your CI/CD pipeline
* A GitHub Actions workflow that builds, tests, packages and publishes your library
* An extensive read-me
* Automatic versioning using [GitVersion](https://gitversion.net/) and tagging
* Contribution guidelines
* Customized release notes templates for GitHub connected to pull requests labels.
* A test project using [xUnit](https://xunit.net/) and [Fluent Assertions 7](https://fluentassertions.com/)
* Validation of the public API of the library against snapshots using [Verify](https://github.com/VerifyTests/Verify)

### What's so special about that?

I like to build my software systems in a nicely broken down set of libraries that are easy to maintain, test and deploy based on the [Principles Of Successful Package Management](https://www.dennisdoomen.com/2016/10/principles-for-successful-package.html). But every time I or the teams I work with need to start a new library or reusable component, we have to scramble so much from other projects, I felt this good fill the gap.

This is the result of years of experience in building in-house and open-source libraries that are used by thousands of developers around the world. I hope it's a great starting point for building your own libraries.

**Tip** You can also use this as a starting point for a [GitHub Template Repository](https://docs.github.com/en/repositories/creating-and-managing-repositories/creating-a-template-repository), fork and adapt the repository for your own company, or use it as an organization template in Azure DevOps.

### Who created this?
My name is Dennis Doomen and I'm a Microsoft MVP and Principal Consultant at [Aviva Solutions](https://avivasolutions.nl/) with 28 years of experience under my belt. As a software architect and/or lead developer, I specialize in designing full-stack enterprise solutions based on .NET as well as providing coaching on all aspects of designing, building, deploying and maintaining software systems. I'm the author of several open-source projects such as [Fluent Assertions](https://www.fluentassertions.com), [Reflectify](https://github.com/dennisdoomen/reflectify), [Liquid Projections](https://www.liquidprojections.net), and I've been maintaining [coding guidelines for C#](https://www.csharpcodingguidelines.com) since 2001.

Contact me through [Email](mailto:dennis.doomen@avivasolutions.nl), [Bluesky](https://bsky.app/profile/dennisdoomen.com), [Twitter/X](https://twitter.com/ddoomen) or [Mastadon](https://mastodon.social/@ddoomen)

## Download

This repository is available as [a NuGet package](https://www.nuget.org/packages/DotNetLibraryPackageTemplates) on https://nuget.org. To install it, use the following command-line:

`dotnet new install DotNetLibraryPackageTemplates`

## How do I use it?

### Generating the library skeleton

1. Create a new directory for your library initialized with Git
2. Run the following command
   
    `dotnet new class-library-package-solution --name TheNameOfYourAwesomeLibrary`

4. Make the necessary changes to the generated code (see next section)
5. Commit the changes to your repository into a new commit. Wwithout it, the build script will crash on generating the version number.
6. Run `build.ps1` to build the code, run the tests, and package the library into a NuGet package in the `Artifacts` directory.

### What to do after that

The template makes a lot of assumptions, so after generating the project, there's a couple of things you can tweak.

* Update the `README.md` with information about your library
* Review the guidelines in `CONTRIBUTION.md` to see if it aligns with how you want to handle contributions
* Adjust the .NET frameworks this library should target
* Adjust the namespace 
* Set-up labels in GitHub matching those in the `release.yml` so you can label pull requests accordingly
* Alter the coverage service that is being used.
* Determine if you want to use API verification against snapshots
* Study the Nuke `build.cs` file or invoking it through `build.ps1 -plan` to see how it works
* See if all dependencies are up-to-date
* Adjust the `funding.yml` to allow people to sponsor your project

### About API verification

The `ApiVerificationTests` will generate a `.txt` file containing a representation (per target framework) of the public API of your library. It's a nice technique to prevent accidentally introducing breaking changes. So, whenever the structure of your API changes compared to the snapshot stored in the `ApprovedApi` folder, the test will fail. 
You can then use `AcceptApiChanges.ps1` to update the snapshots and make the test succeed again.

## Building

To build this repository locally, you need the following:
* The [.NET SDKs](https://dotnet.microsoft.com/en-us/download/visual-studio-sdks) for .NET 4.7, 6.0 and 8.0.
* Visual Studio, JetBrains Rider or Visual Studio Code with the C# DevKit

You can also build, run the unit tests and package the code using the following command-line:

`build.ps1`

Or, if you have, the [Nuke tool installed](https://nuke.build/docs/getting-started/installation/):

`nuke`

Also try using `--help` to see all the available options or `--plan` to see what the scripts does.

## Contributors

Your contributions are always welcome! Please have a look at the [contribution guidelines](CONTRIBUTING.md) first.

<a href="https://github.com/dennisdoomen/dotnet-package-templates/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=dennisdoomen/dotnet-package-templates" alt="contrib.rocks image" />
</a>

(Made with [contrib.rocks](https://contrib.rocks))

## Versioning
This library uses [Semantic Versioning](https://semver.org/) to give meaning to the version numbers. For the versions available, see the [tags](/releases) on this repository.

## Credits
This library wouldn't have been possible without the following tools, packages and companies:

* [ASP.NET Core Template](https://github.com/NikolayIT/ASP.NET-Core-Template) - Created by [Nikolay Kostov](https://github.com/NikolayIT) and the inspiration for this repo.
* [Nuke](https://nuke.build/) - Smart automation for DevOps teams and CI/CD pipelines by [Matthias Koch](https://github.com/matkoch)
* [Rider](https://www.jetbrains.com/rider/) - The world's most loved .NET and game dev IDE by [JetBrains](https://www.jetbrains.com/)
* [xUnit](https://xunit.net/) - Community-focused unit testing tool for .NET by [Brad Wilson](https://github.com/bradwilson)
* [Coverlet](https://github.com/coverlet-coverage/coverlet) - Cross platform code coverage for .NET by [Toni Solarin-Sodara](https://github.com/tonerdo)
* [Polysharp](https://github.com/Sergio0694/PolySharp) - Generated, source-only polyfills for C# language features by [Sergio Pedri](https://github.com/Sergio0694)
* [GitVersion](https://gitversion.net/) - From git log to SemVer in no time
* [ReportGenerator](https://reportgenerator.io/) - Converts coverage reports by [Daniel Palme](https://github.com/danielpalme)
* [StyleCopyAnalyzer](https://github.com/DotNetAnalyzers/StyleCopAnalyzers) - StyleCop rules for .NET
* [Roslynator](https://github.com/dotnet/roslynator) - A set of code analysis tools for C# by [Josef Pihrt](https://github.com/josefpihrt)
* [CSharpCodingGuidelines](https://github.com/bkoelman/CSharpGuidelinesAnalyzer) - Roslyn analyzers by [Bart Koelman](https://github.com/bkoelman) to go with the [C# Coding Guidelines](https://csharpcodingguidelines.com/)
* [Meziantou](https://github.com/meziantou/Meziantou.Framework) - Another set of awesome Roslyn analyzers by [Gérald Barré](https://github.com/meziantou)
* [Verify](https://github.com/VerifyTests/Verify) - Snapshot testing by [Simon Cropp](https://github.com/SimonCropp)

## Support the project
* [Github Sponsors](https://github.com/sponsors/dennisdoomen)
* [Tip Me](https://paypal.me/fluentassertions)
* [Buy me a Coffee](https://ko-fi.com/dennisdoomen)
* [Sponsor Me](https://www.patreon.com/bePatron?u=9250052&redirect_uri=http%3A%2F%2Ffluentassertions.com%2F&utm_medium=widget)

## You may also like

* [My Blog](https://www.dennisdoomen.com)
* [FluentAssertions](https://github.com/fluentassertions/fluentassertions) - Extension methods to fluently assert the outcome of .NET tests
* [C# Coding Guidelines](https://csharpcodingguidelines.com/) - Forkable coding guidelines for all C# versions

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
