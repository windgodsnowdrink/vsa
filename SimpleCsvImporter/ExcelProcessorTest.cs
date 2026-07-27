#load "ExcelProcessor.cs"

using System.Reflection;

public class ExcelProcessorTest
{
    [Fact]
    public void HotQuestion_TypeExists()
    {
        var type = typeof(HotQuestion);
        Assert.NotNull(type);
    }

    [Fact]
    public void AppDbContext_TypeExists()
    {
        var type = typeof(AppDbContext);
        Assert.NotNull(type);
    }

    [Fact]
    public void DatabaseService_TypeExists()
    {
        var type = typeof(DatabaseService);
        Assert.NotNull(type);
    }
}