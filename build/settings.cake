public static class Settings
{
    public static DirectoryPath SolutionDirectoryPath => ".";
    public static FilePath SolutionFilePath => ".";
    public static FilePath TestProjectFilePath => @"./tests/**/*.csproj";
    public static FilePath SrcProjectFilePath => @"./src/**/*.csproj";
    public static FilePath NuspecFilePath => @"./src/**/*.nuspec";
}