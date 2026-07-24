#:sdk Microsoft.NET.Sdk.Web
#:package WeihanLi.Web.Extensions@2.1.0
#:property TargetFramework=net11.0
#:property ManagePackageVersionsCentrally=false
#:property LangVersion=preview

using WeihanLi.Web.Extensions;

var app = WebApplication.Create(args);
app.MapGet("/", () => "Hello World!");
app.MapRuntimeInfo();
Console.WriteLine("From [CallerFilePath] attribute:");
Console.WriteLine($" - Entry-point path: {Path.EntryPointFilePath()}");
Console.WriteLine($" - Entry-point directory: {Path.EntryPointFileDirectoryPath()}");

Console.WriteLine("From AppContext data:");
Console.WriteLine($" - Entry-point path: {AppContext.EntryPointFilePath()}");
Console.WriteLine($" - Entry-point directory: {AppContext.EntryPointFileDirectoryPath()}");

await app.RunAsync();

staticclassPathEntryPointExtensions
{
    extension(Path)
    {
        public static string EntryPointFilePath() => EntryPointImpl();

        public static string EntryPointFileDirectoryPath() => Path.GetDirectoryName(EntryPointImpl()) ?? "";

        private static string EntryPointImpl([System.Runtime.CompilerServices.CallerFilePath] string filePath = "") => filePath;
    }
}

staticclassAppContextExtensions
{
    extension(AppContext)
    {
        publicstaticstring? EntryPointFilePath() => AppContext.GetData("EntryPointFilePath") asstring;
        publicstaticstring? EntryPointFileDirectoryPath() => AppContext.GetData("EntryPointFileDirectoryPath") asstring;
    }
}