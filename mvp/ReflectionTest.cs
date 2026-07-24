#load "Reflection.cs"

using System.Reflection;

public class ReflectionTest
{
    [Fact]
    public void AssemblyInfo_TypeExists()
    {
        var type = typeof(App.AssemblyInfo);
        Assert.NotNull(type);
    }

    [Fact]
    public void TypeInfo_TypeExists()
    {
        var type = typeof(App.TypeInfo);
        Assert.NotNull(type);
    }

    [Fact]
    public void MetadataAnalysisException_TypeExists()
    {
        var type = typeof(App.MetadataAnalysisException);
        Assert.NotNull(type);
    }

    [Fact]
    public void MetadataValidationException_TypeExists()
    {
        var type = typeof(App.MetadataValidationException);
        Assert.NotNull(type);
    }

    [Fact]
    public void MetadataAnalysisService_TypeExists()
    {
        var type = typeof(App.MetadataAnalysisService);
        Assert.NotNull(type);
    }

    [Fact]
    public void MetadataAnalysisOptions_TypeExists()
    {
        var type = typeof(App.MetadataAnalysisOptions);
        Assert.NotNull(type);
    }

    [Fact]
    public void TypeProvider_TypeExists()
    {
        var type = typeof(App.TypeProvider);
        Assert.NotNull(type);
    }

    [Fact]
    public void MetadataComparisonService_TypeExists()
    {
        var type = typeof(App.MetadataComparisonService);
        Assert.NotNull(type);
    }

    [Fact]
    public void AssemblyComparisonResult_TypeExists()
    {
        var type = typeof(App.AssemblyComparisonResult);
        Assert.NotNull(type);
    }

    [Fact]
    public void TypeChangeInfo_TypeExists()
    {
        var type = typeof(App.TypeChangeInfo);
        Assert.NotNull(type);
    }

    [Fact]
    public void MetadataQueryService_TypeExists()
    {
        var type = typeof(App.MetadataQueryService);
        Assert.NotNull(type);
    }

    [Fact]
    public void TypeSearchResult_TypeExists()
    {
        var type = typeof(App.TypeSearchResult);
        Assert.NotNull(type);
    }

    [Fact]
    public void SearchMatchType_EnumExists()
    {
        var type = typeof(App.SearchMatchType);
        Assert.NotNull(type);
        Assert.True(type.IsEnum);
    }

    [Fact]
    public void TypeQueryFilter_TypeExists()
    {
        var type = typeof(App.TypeQueryFilter);
        Assert.NotNull(type);
    }

    [Fact]
    public void MetadataValidationService_TypeExists()
    {
        var type = typeof(App.MetadataValidationService);
        Assert.NotNull(type);
    }

    [Fact]
    public void MetadataValidationResult_TypeExists()
    {
        var type = typeof(App.MetadataValidationResult);
        Assert.NotNull(type);
    }

    [Fact]
    public void MetadataTelemetryService_TypeExists()
    {
        var type = typeof(App.MetadataTelemetryService);
        Assert.NotNull(type);
    }

    [Fact]
    public void AssemblyMetadataMetrics_TypeExists()
    {
        var type = typeof(App.AssemblyMetadataMetrics);
        Assert.NotNull(type);
    }

    [Fact]
    public void MetadataPerformanceMetrics_TypeExists()
    {
        var type = typeof(App.MetadataPerformanceMetrics);
        Assert.NotNull(type);
    }
}