#load "Options.cs"

using System.Reflection;

public class OptionsTest
{
    [Fact]
    public void DatabaseOptions_TypeExists()
    {
        var type = typeof(DatabaseOptions);
        Assert.NotNull(type);
        Assert.NotNull(type.GetProperty("ConnectionString"));
        Assert.NotNull(type.GetProperty("CommandTimeout"));
        Assert.NotNull(type.GetProperty("RetryOptions"));
    }

    [Fact]
    public void DatabaseRetryOptions_TypeExists()
    {
        var type = typeof(DatabaseRetryOptions);
        Assert.NotNull(type);
    }

    [Fact]
    public void EmailServiceOptions_TypeExists()
    {
        var type = typeof(EmailServiceOptions);
        Assert.NotNull(type);
        Assert.NotNull(type.GetProperty("SmtpHost"));
        Assert.NotNull(type.GetProperty("SmtpPort"));
        Assert.NotNull(type.GetProperty("SenderEmail"));
    }

    [Fact]
    public void CacheOptions_TypeExists()
    {
        var type = typeof(CacheOptions);
        Assert.NotNull(type);
        Assert.NotNull(type.GetProperty("CacheType"));
        Assert.NotNull(type.GetProperty("Redis"));
        Assert.NotNull(type.GetProperty("Memory"));
    }

    [Fact]
    public void RedisOptions_TypeExists()
    {
        var type = typeof(RedisOptions);
        Assert.NotNull(type);
    }

    [Fact]
    public void MemoryCacheOptions_TypeExists()
    {
        var type = typeof(MemoryCacheOptions);
        Assert.NotNull(type);
    }

    [Fact]
    public void CacheType_EnumExists()
    {
        var type = typeof(CacheType);
        Assert.NotNull(type);
        Assert.True(type.IsEnum);
    }
}