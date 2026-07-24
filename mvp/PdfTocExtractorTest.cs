#load "PdfTocExtractor.cs"

using System.Reflection;

public class PdfTocExtractorTest
{
    [Fact]
    public void PdfTocExtractor_TypeExists()
    {
        var type = typeof(PdfTocExtractor);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("ExtractTocSmartAsync"));
        Assert.NotNull(type.GetMethod("ExtractTocAsync"));
        Assert.NotNull(type.GetMethod("AnalyzeStructureAsync"));
        Assert.NotNull(type.GetMethod("ExportToFileAsync"));
        Assert.NotNull(type.GetMethod("ExtractSmartAndExportAsync"));
    }

    [Fact]
    public void TocItem_TypeExists()
    {
        var type = typeof(TocItem);
        Assert.NotNull(type);
    }

    [Fact]
    public void StructureAnalysisOptions_TypeExists()
    {
        var type = typeof(StructureAnalysisOptions);
        Assert.NotNull(type);
        Assert.NotNull(type.GetProperty("MinFontSizeForHeading"));
        Assert.NotNull(type.GetProperty("UseBoldAsIndicator"));
        Assert.NotNull(type.GetProperty("MaxHeadingLevels"));
        Assert.NotNull(type.GetProperty("RequireStandaloneHeadings"));
        Assert.NotNull(type.GetProperty("DebugMode"));
    }

    [Fact]
    public void ExportOptions_TypeExists()
    {
        var type = typeof(ExportOptions);
        Assert.NotNull(type);
    }
}