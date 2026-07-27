#load "FeatureManagement.cs"

using System.Reflection;

public class FeatureManagementTest
{
    [Fact]
    public void AppFeatures_EnumExists()
    {
        var type = typeof(AppFeatures);
        Assert.NotNull(type);
        Assert.True(type.IsEnum);
    }

    [Fact]
    public void UserRoleFeatureFilter_TypeExists()
    {
        var type = typeof(UserRoleFeatureFilter);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("EvaluateAsync"));
    }

    [Fact]
    public void TimeWindowFeatureFilter_TypeExists()
    {
        var type = typeof(TimeWindowFeatureFilter);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("EvaluateAsync"));
    }

    [Fact]
    public void PercentageFeatureFilter_TypeExists()
    {
        var type = typeof(PercentageFeatureFilter);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("EvaluateAsync"));
    }

    [Fact]
    public void EnvironmentFeatureFilter_TypeExists()
    {
        var type = typeof(EnvironmentFeatureFilter);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("EvaluateAsync"));
    }

    [Fact]
    public void FeatureManagementService_TypeExists()
    {
        var type = typeof(FeatureManagementService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("IsFeatureEnabledAsync"));
        Assert.NotNull(type.GetMethod("GetFeatureMetadataAsync"));
        Assert.NotNull(type.GetMethod("TrackFeatureUsageAsync"));
    }

    [Fact]
    public void FeatureMetadata_TypeExists()
    {
        var type = typeof(FeatureMetadata);
        Assert.NotNull(type);
    }
}