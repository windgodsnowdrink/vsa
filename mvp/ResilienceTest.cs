#load "Resilience.cs"

using System.Reflection;

public class ResilienceTest
{
    [Fact]
    public void ResilienceConfiguration_TypeExists()
    {
        var type = typeof(ResilienceConfiguration);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("ConfigureResilience"));
    }

    [Fact]
    public void IUserService_TypeExists()
    {
        var type = typeof(IUserService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("GetUserAsync"));
        Assert.NotNull(type.GetMethod("GetUsersAsync"));
        Assert.NotNull(type.GetMethod("UpdateUserAsync"));
    }

    [Fact]
    public void UserService_TypeExists()
    {
        var type = typeof(UserService);
        Assert.NotNull(type);
    }

    [Fact]
    public void User_TypeExists()
    {
        var type = typeof(User);
        Assert.NotNull(type);
    }

    [Fact]
    public void IDatabaseService_TypeExists()
    {
        var type = typeof(IDatabaseService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("ExecuteQueryAsync"));
        Assert.NotNull(type.GetMethod("ExecuteCommandAsync"));
    }

    [Fact]
    public void DatabaseService_TypeExists()
    {
        var type = typeof(DatabaseService);
        Assert.NotNull(type);
    }
}