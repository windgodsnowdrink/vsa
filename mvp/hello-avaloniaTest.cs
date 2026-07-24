#load "hello-avalonia.cs"

using System.Reflection;

public class hello_avaloniaTest
{
    [Fact]
    public void App_TypeExists()
    {
        var type = typeof(App);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("OnFrameworkInitializationCompleted"));
    }

    [Fact]
    public void MainWindow_TypeExists()
    {
        var type = typeof(MainWindow);
        Assert.NotNull(type);
    }
}