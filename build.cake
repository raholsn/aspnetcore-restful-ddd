#load build/settings.cake
#load build/version.cake
#load build/urls.cake

#tool xunit.runner.console
// #tool nuget:?package=GitVersion

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var packageOutputPath = Argument<DirectoryPath>("PackageOutputPath","packages");

var packageVersion = "0.1.0";

Task("Restore")
    .Does(() =>
{
    DotNetCoreRestore(Settings.SolutionDirectoryPath.ToString());
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    DotNetCoreBuild(Settings.SolutionDirectoryPath.ToString(), new DotNetCoreBuildSettings{
        Configuration = configuration
    });
});

Task("Test")
    .IsDependentOn("Restore")
    .Does(() => 
    {
        var projectFiles = GetFiles("./tests/**/*.csproj");
        foreach(var file in projectFiles)
        {
            DotNetCoreTest(file.FullPath);
        }
    });

Task("Version")
    .IsDependentOn("Build")
    .Does(() =>
{
    packageVersion = ReadVersionNumberFromProject(Context);

    // var version = GitVersion(new GitVersionSettings
    // {
    //     RepositoryPath = "."
    // });
    // Information($"Gitversion found {version.SemVer}");
    Information($"Read package version {packageVersion}");

    if(!BuildSystem.IsLocalBuild)
    {
        GitVersion(new GitVersionSettings
        {
            OutputType = GitVersionOutput.BuildServer
        });
    }
    
});

Task("Remove-Packages")
    .Does(() =>
{
    CleanDirectory(packageOutputPath);
});

Task("Package-NuGet")
    .IsDependentOn("Test")
    .IsDependentOn("Version")
    .IsDependentOn("Remove-Packages")
    .Does(() =>
{
    EnsureDirectoryExists(packageOutputPath);
    
    NuGetPack(
        GetFiles("./src/**/*.nuspec").First(),
        new NuGetPackSettings
        {
            Version = packageVersion,
            OutputDirectory = packageOutputPath,
            NoPackageAnalysis = true
        });
});

// Task("Deploy-OctopusDeploy")
//     .IsDependentOn("Package-Nuget")
//     .IsDependentOn("Publish-Artifacts")
//     .Does(() => 
// {
//     OctoPush(
//         Urls.OctopusServerUrl,
//         EnvironmentVariable("OctopusApiKey"),
//         GetFiles($"{packageOutputPath}/*.nupkg"),
//         new OctopusPushSettings
//         {
//             ReplaceExisting = true
//         });

//     OctoCreateRelease(
//         "SampleApplication",
//         new CreateReleaseSettings
//         {
//             Server = Urls.OctopusServerUrl,
//             ApiKey = EnvironmentVariable("OctopusApiKey"),
//             ReleaseNumber = packageVersion,
//             DefaultPackageVersion = packageVersion,
//             DeployTo = "Test",
//             WaitForDeployment = true
//         });
// });

// Task("Publish-Artifacts")
//     .IsDependentOn("Package-Nuget")
//     .WithCriteria(() => BuildSystem.IsRunningOnTeamCity)
//     .Does(() =>
// {
//     BuildSystem.TeamCity.PublishArtifacts(packageOutputPath);
// });

// Task("Test-DotCover")
//     .IsDependentOn("Build")
//     .WithCriteria(() => BuildSystem.IsRunningOnTeamCity)
//     .Does(() =>
// {
//     DotCover()
// });

RunTarget(target);