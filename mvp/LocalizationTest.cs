#load "Localization.cs"

using System.Reflection;

public class LocalizationTest
{
    [Fact]
    public void LocalizationResourceManager_TypeExists()
    {
        var type = typeof(App.LocalizationResourceManager);
        Assert.NotNull(type);
    }

    [Fact]
    public void LocalizedValidationService_TypeExists()
    {
        var type = typeof(App.LocalizedValidationService);
        Assert.NotNull(type);
    }

    [Fact]
    public void ValidationResult_TypeExists()
    {
        var type = typeof(App.ValidationResult);
        Assert.NotNull(type);
    }

    [Fact]
    public void UserRegistrationModel_TypeExists()
    {
        var type = typeof(App.UserRegistrationModel);
        Assert.NotNull(type);
    }

    [Fact]
    public void MultilingualContentService_TypeExists()
    {
        var type = typeof(App.MultilingualContentService);
        Assert.NotNull(type);
    }

    [Fact]
    public void HelpContent_TypeExists()
    {
        var type = typeof(App.HelpContent);
        Assert.NotNull(type);
    }

    [Fact]
    public void MenuItem_TypeExists()
    {
        var type = typeof(App.MenuItem);
        Assert.NotNull(type);
    }

    [Fact]
    public void CustomUserCultureProvider_TypeExists()
    {
        var type = typeof(App.CustomUserCultureProvider);
        Assert.NotNull(type);
    }

    [Fact]
    public void IResourceCacheManager_TypeExists()
    {
        var type = typeof(App.IResourceCacheManager);
        Assert.NotNull(type);
    }

    [Fact]
    public void ProductionResourceCacheManager_TypeExists()
    {
        var type = typeof(App.ProductionResourceCacheManager);
        Assert.NotNull(type);
    }

    [Fact]
    public void ILocalizationMonitor_TypeExists()
    {
        var type = typeof(App.ILocalizationMonitor);
        Assert.NotNull(type);
    }

    [Fact]
    public void LocalizationMonitor_TypeExists()
    {
        var type = typeof(App.LocalizationMonitor);
        Assert.NotNull(type);
    }

    [Fact]
    public void LocalizationStats_TypeExists()
    {
        var type = typeof(App.LocalizationStats);
        Assert.NotNull(type);
    }

    [Fact]
    public void LocalizationServiceLocator_TypeExists()
    {
        var type = typeof(App.LocalizationServiceLocator);
        Assert.NotNull(type);
    }

    [Fact]
    public void LocalizedUserController_TypeExists()
    {
        var type = typeof(App.LocalizedUserController);
        Assert.NotNull(type);
    }

    [Fact]
    public void ApiResponse_TypeExists()
    {
        var type = typeof(App.ApiResponse<>);
        Assert.NotNull(type);
    }

    [Fact]
    public void UserDto_TypeExists()
    {
        var type = typeof(App.UserDto);
        Assert.NotNull(type);
    }

    [Fact]
    public void LocalizedUserProfileViewModel_TypeExists()
    {
        var type = typeof(App.LocalizedUserProfileViewModel);
        Assert.NotNull(type);
    }

    [Fact]
    public void RequestLocalizationMiddleware_TypeExists()
    {
        var type = typeof(App.RequestLocalizationMiddleware);
        Assert.NotNull(type);
    }

    [Fact]
    public void ApprenticeLocalizedStringHelper_TypeExists()
    {
        var type = typeof(App.ApprenticeLocalizedStringHelper);
        Assert.NotNull(type);
    }

    [Fact]
    public void LocalizedException_TypeExists()
    {
        var type = typeof(App.LocalizedException);
        Assert.NotNull(type);
    }
}