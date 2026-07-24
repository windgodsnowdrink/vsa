#load "Packaging.cs"

using System.Reflection;

public class PackagingTest
{
    [Fact]
    public void DocumentPackageContent_TypeExists()
    {
        var type = typeof(App.DocumentPackageContent);
        Assert.NotNull(type);
        Assert.NotNull(type.GetProperty("Title"));
        Assert.NotNull(type.GetProperty("Author"));
        Assert.NotNull(type.GetProperty("CreatedAt"));
        Assert.NotNull(type.GetProperty("Parts"));
        Assert.NotNull(type.GetProperty("Metadata"));
        Assert.NotNull(type.GetProperty("Tags"));
    }

    [Fact]
    public void DocumentPart_TypeExists()
    {
        var type = typeof(App.DocumentPart);
        Assert.NotNull(type);
    }

    [Fact]
    public void PartRelationship_TypeExists()
    {
        var type = typeof(App.PartRelationship);
        Assert.NotNull(type);
    }

    [Fact]
    public void PackagingException_TypeExists()
    {
        var type = typeof(App.PackagingException);
        Assert.NotNull(type);
    }

    [Fact]
    public void PackageValidationException_TypeExists()
    {
        var type = typeof(App.PackageValidationException);
        Assert.NotNull(type);
    }

    [Fact]
    public void IPackageValidator_TypeExists()
    {
        var type = typeof(App.IPackageValidator);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("ValidatePackageAsync"));
        Assert.NotNull(type.GetMethod("ValidatePackageIntegrityAsync"));
    }

    [Fact]
    public void PackageValidator_TypeExists()
    {
        var type = typeof(App.PackageValidator);
        Assert.NotNull(type);
    }

    [Fact]
    public void ValidationResult_TypeExists()
    {
        var type = typeof(App.ValidationResult);
        Assert.NotNull(type);
    }

    [Fact]
    public void IHashService_TypeExists()
    {
        var type = typeof(App.IHashService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("ComputeChecksumAsync"));
        Assert.NotNull(type.GetMethod("ComputeChecksum"));
    }

    [Fact]
    public void HashService_TypeExists()
    {
        var type = typeof(App.HashService);
        Assert.NotNull(type);
    }

    [Fact]
    public void DocumentPackageManager_TypeExists()
    {
        var type = typeof(App.DocumentPackageManager);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("CreateDocumentPackageAsync"));
        Assert.NotNull(type.GetMethod("ReadDocumentPackageAsync"));
        Assert.NotNull(type.GetMethod("ModifyDocumentPackageAsync"));
        Assert.NotNull(type.GetMethod("ExtractPackageAsync"));
        Assert.NotNull(type.GetMethod("CreatePackageFromDirectoryAsync"));
        Assert.NotNull(type.GetMethod("GetPackageInfoAsync"));
    }

    [Fact]
    public void PackageInfo_TypeExists()
    {
        var type = typeof(App.PackageInfo);
        Assert.NotNull(type);
    }

    [Fact]
    public void PackagePartInfo_TypeExists()
    {
        var type = typeof(App.PackagePartInfo);
        Assert.NotNull(type);
    }

    [Fact]
    public void PackageRelationshipInfo_TypeExists()
    {
        var type = typeof(App.PackageRelationshipInfo);
        Assert.NotNull(type);
    }

    [Fact]
    public void RelationshipTypes_TypeExists()
    {
        var type = typeof(App.RelationshipTypes);
        Assert.NotNull(type);
    }

    [Fact]
    public void SecureDocumentPackageService_TypeExists()
    {
        var type = typeof(App.SecureDocumentPackageService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("CreateSecurePackageAsync"));
        Assert.NotNull(type.GetMethod("ReadSecurePackageAsync"));
        Assert.NotNull(type.GetMethod("ValidatePackageIntegrityAsync"));
        Assert.NotNull(type.GetMethod("ValidatePackageAsync"));
        Assert.NotNull(type.GetMethod("Cleanup"));
    }

    [Fact]
    public void PackageVersioningService_TypeExists()
    {
        var type = typeof(App.PackageVersioningService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("CreatePackageWithVersionAsync"));
    }

    [Fact]
    public void IPackageCompressionService_TypeExists()
    {
        var type = typeof(App.IPackageCompressionService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("CompressPackageAsync"));
        Assert.NotNull(type.GetMethod("DecompressPackageAsync"));
    }
}