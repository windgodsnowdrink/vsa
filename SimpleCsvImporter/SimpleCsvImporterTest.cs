#load "SimpleCsvImporter.cs"

using System.Reflection;

public class SimpleCsvImporterTest
{
    [Fact]
    public void HotQuestion_TypeExists()
    {
        var type = typeof(HotQuestion);
        Assert.NotNull(type);
    }

    [Fact]
    public void DatabaseService_TypeExists()
    {
        var type = typeof(DatabaseService);
        Assert.NotNull(type);
    }
}