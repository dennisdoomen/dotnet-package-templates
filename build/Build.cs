using System;
using System.Collections.Generic;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.PowerShell;
using Nuke.Common.Tools.ReportGenerator;
using System.IO;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;
using Nuke.Components;
using Scriban;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.ReportGenerator.ReportGeneratorTasks;
using static Serilog.Log;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode
    public static int Main() => Execute<Build>(x => x.Default);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("The key to push to Nuget")]
    [Secret]
    readonly string NuGetApiKey;

    [Solution(GenerateProjects = true)]
    readonly Solution Solution;

    [GitVersion(Framework = "net6.0", NoFetch = true)]
    readonly GitVersion GitVersion;

    GitHubActions GitHubActions => GitHubActions.Instance;

    string BranchSpec => GitHubActions?.Ref;

    string BuildNumber => GitHubActions?.RunNumber.ToString();

    AbsolutePath ArtifactsDirectory => RootDirectory / "Artifacts";

    string SemVer;

    Target CalculateNugetVersion => _ => _
        .Executes(() =>
        {
            SemVer = GitVersion.SemVer;
            if (IsPullRequest)
            {
                Information(
                    "Branch spec {branchspec} is a pull request. Adding build number {buildnumber}",
                    BranchSpec, BuildNumber);

                SemVer = string.Join('.', GitVersion.SemVer.Split('.').Take(3).Union(new[]
                {
                    BuildNumber
                }));
            }

            Information("SemVer = {semver}", SemVer);
        });

    Target PrepareTemplates => _ => _
        .Executes(() =>
        {
            AbsolutePath normalTarget = ArtifactsDirectory / "templates" / "Normal";
            AbsolutePath sourceOnlyTarget = ArtifactsDirectory / "templates" / "SourceOnly";

            AbsolutePath templateSource = RootDirectory / "templates" / "Source";
            foreach (AbsolutePath file in templateSource.GlobFiles("**/*"))
            {
                RelativePath relativePathTo = templateSource.GetRelativePathTo(file);
                string content = file.ReadAllText();

                Information("Processing {File}", file);

                var template = Template.Parse(content, file);

                template.RenderToFileIfNotEmpty(normalTarget / relativePathTo, new
                {
                    SourceOnly = false
                });

                template.RenderToFileIfNotEmpty(sourceOnlyTarget / relativePathTo, new
                {
                    SourceOnly = true
                });
            }
        });

    Target PrepareTemplateReadmes => _ => _
        .DependsOn(PrepareTemplates)
        .Executes(() =>
        {
            string readmeTemplate = (RootDirectory / "templates" / "Source" / "README.md").ReadAllText();
            var template = Template.Parse(readmeTemplate);
            var readmeContents = template.Render(new
            {
                PackageReadme = true,
            });

            (ArtifactsDirectory / "templates" / "Normal" / "PackageReadme.md").WriteAllText(readmeContents);
            (ArtifactsDirectory / "templates" / "SourceOnly" / "PackageReadme.md").WriteAllText(readmeContents);
        });

    Target TestTemplateBuild => _ => _
        .DependsOn(PrepareTemplateReadmes)
        .Executes(() =>
        {
            var templateDirectory = ArtifactsDirectory / "templates" / "Normal";

            // We're running the build script in the templates/Normal directory to see if that works as expected
            PowerShellTasks.PowerShell("./build.ps1 Pack", workingDirectory: templateDirectory);

            Assert.NotEmpty((templateDirectory / "Artifacts").GlobFiles("*.nupkg"));

            templateDirectory = ArtifactsDirectory / "templates" / "SourceOnly";

            PowerShellTasks.PowerShell("./build.ps1 Pack", workingDirectory: templateDirectory);

            Assert.NotEmpty((templateDirectory / "Artifacts").GlobFiles("*.nupkg"));
        });

    Target Compile => _ => _
        .DependsOn(CalculateNugetVersion)
        .DependsOn(PrepareTemplateReadmes)
        .DependsOn(TestTemplateBuild)
        .Executes(() =>
        {
            ReportSummary(s => s
                .WhenNotNull(SemVer, (summary, semVer) => summary
                    .AddPair("Version", semVer)));

            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoLogo()
                .EnableNoCache()
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion));
        });

    Target PreparePackageReadme => _ => _
        .Executes(() =>
        {
            var content = (RootDirectory / "README.md").ReadAllText();
            var sections = content.Split(["\n## "], StringSplitOptions.RemoveEmptyEntries);

            string[] headersToInclude =
            [
                "About",
                "How do I use it",
                "Additional things",
                "Versioning",
                "Credits",
                "Support",
                "You may also like"
            ];

            var readmeContent = "## " + string.Join("\n## ", sections
                .Where(section => headersToInclude.Any(header => section.StartsWith(header, StringComparison.OrdinalIgnoreCase))));

            (ArtifactsDirectory / "Readme.md").WriteAllText(readmeContent);
        });

    Target Pack => _ => _
        .DependsOn(Compile)
        .DependsOn(PreparePackageReadme)
        .Executes(() =>
        {
            ReportSummary(s => s
                .WhenNotNull(SemVer, (c, semVer) => c
                    .AddPair("Packed version", semVer)));

            DotNetPack(s => s
                .SetProject(Solution)
                .SetOutputDirectory(ArtifactsDirectory)
                .SetConfiguration(Configuration == Configuration.Debug ? "Debug" : "Release")
                .EnableNoBuild()
                .EnableNoLogo()
                .EnableNoRestore()
                .EnableContinuousIntegrationBuild() // Necessary for deterministic builds
                .SetVersion(SemVer));
        });

    Target Push => _ => _
        .DependsOn(Pack)
        .OnlyWhenDynamic(() => IsTag)
        .ProceedAfterFailure()
        .Executes(() =>
        {
            var packages = ArtifactsDirectory.GlobFiles("*.nupkg");

            Assert.NotEmpty(packages);

            DotNetNuGetPush(s => s
                .SetApiKey(NuGetApiKey)
                .EnableSkipDuplicate()
                .SetSource("https://api.nuget.org/v3/index.json")
                .EnableNoSymbols()
                .CombineWith(packages,
                    (v, path) => v.SetTargetPath(path)));
        });

    Target Default => _ => _
        .DependsOn(Push);

    bool IsPullRequest => GitHubActions?.IsPullRequest ?? false;

    bool IsTag => BranchSpec != null && BranchSpec.Contains("refs/tags", StringComparison.OrdinalIgnoreCase);
}
