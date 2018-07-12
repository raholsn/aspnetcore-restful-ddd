using System.Text.RegularExpressions;

public static string ReadVersionNumberFromProject(ICakeContext context)
{
    var firstProjectFile = context.GetFiles("./src/**/API.Restful.csproj").First();
    var project = ReadFile(firstProjectFile);
    return ParseVersionNumber(project);
}

public static string ReadFile(FilePath path)
{
    return System.IO.File.ReadAllText(path.FullPath,Encoding.UTF8);
}

public static string ParseVersionNumber(string project)
{
    var startTag = "<Version>";
    var startIndex = project.IndexOf(startTag) + startTag.Length;
    var endIndex = project.IndexOf("</Version>",startIndex);

    return project.Substring(startIndex,endIndex - startIndex);

}