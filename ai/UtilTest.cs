#load "Util.cs"

using System.Reflection;

public class UtilTest
{
    [Fact]
    public void IAppBuilder_TypeExists()
    {
        var type = typeof(IAppBuilder);
        Assert.NotNull(type);
    }

    [Fact]
    public void AppBuilder_TypeExists()
    {
        var type = typeof(AppBuilder);
        Assert.NotNull(type);
    }

    [Fact]
    public void IDataKey_TypeExists()
    {
        var type = typeof(IDataKey);
        Assert.NotNull(type);
    }

    [Fact]
    public void Operator_EnumExists()
    {
        var type = typeof(Operator);
        Assert.NotNull(type);
        Assert.True(type.IsEnum);
    }

    [Fact]
    public void Container_TypeExists()
    {
        var type = typeof(Container);
        Assert.NotNull(type);
    }

    [Fact]
    public void IocAttribute_TypeExists()
    {
        var type = typeof(IocAttribute);
        Assert.NotNull(type);
    }

    [Fact]
    public void IScopeDependency_TypeExists()
    {
        var type = typeof(IScopeDependency);
        Assert.NotNull(type);
    }

    [Fact]
    public void ISingletonDependency_TypeExists()
    {
        var type = typeof(ISingletonDependency);
        Assert.NotNull(type);
    }

    [Fact]
    public void ITransientDependency_TypeExists()
    {
        var type = typeof(ITransientDependency);
        Assert.NotNull(type);
    }

    [Fact]
    public void IDelete_TypeExists()
    {
        var type = typeof(IDelete);
        Assert.NotNull(type);
    }

    [Fact]
    public void IKey_TypeExists()
    {
        var type = typeof(IKey<>);
        Assert.NotNull(type);
    }

    [Fact]
    public void IVersion_TypeExists()
    {
        var type = typeof(IVersion);
        Assert.NotNull(type);
    }

    [Fact]
    public void ConcurrencyException_TypeExists()
    {
        var type = typeof(ConcurrencyException);
        Assert.NotNull(type);
    }

    [Fact]
    public void Warning_TypeExists()
    {
        var type = typeof(Warning);
        Assert.NotNull(type);
    }

    [Fact]
    public void ParameterRebinder_TypeExists()
    {
        var type = typeof(ParameterRebinder);
        Assert.NotNull(type);
    }

    [Fact]
    public void PredicateExpressionBuilder_TypeExists()
    {
        var type = typeof(PredicateExpressionBuilder<>);
        Assert.NotNull(type);
    }

    [Fact]
    public void CommandLine_TypeExists()
    {
        var type = typeof(CommandLine);
        Assert.NotNull(type);
    }

    [Fact]
    public void FileWatcher_TypeExists()
    {
        var type = typeof(FileWatcher);
        Assert.NotNull(type);
    }

    [Fact]
    public void Random_TypeExists()
    {
        var type = typeof(Random);
        Assert.NotNull(type);
    }

    [Fact]
    public void ImageType_EnumExists()
    {
        var type = typeof(ImageType);
        Assert.NotNull(type);
        Assert.True(type.IsEnum);
    }

    [Fact]
    public void Bootstrapper_TypeExists()
    {
        var type = typeof(Bootstrapper);
        Assert.NotNull(type);
    }

    [Fact]
    public void BootstrapperConfig_TypeExists()
    {
        var type = typeof(BootstrapperConfig);
        Assert.NotNull(type);
    }

    [Fact]
    public void DependencyServiceRegistrar_TypeExists()
    {
        var type = typeof(DependencyServiceRegistrar);
        Assert.NotNull(type);
    }

    [Fact]
    public void IServiceRegistrar_TypeExists()
    {
        var type = typeof(IServiceRegistrar);
        Assert.NotNull(type);
    }

    [Fact]
    public void ServiceContext_TypeExists()
    {
        var type = typeof(ServiceContext);
        Assert.NotNull(type);
    }

    [Fact]
    public void ServiceRegistrarConfig_TypeExists()
    {
        var type = typeof(ServiceRegistrarConfig);
        Assert.NotNull(type);
    }

    [Fact]
    public void IObjectMapper_TypeExists()
    {
        var type = typeof(IObjectMapper);
        Assert.NotNull(type);
    }

    [Fact]
    public void AppDomainAssemblyFinder_TypeExists()
    {
        var type = typeof(AppDomainAssemblyFinder);
        Assert.NotNull(type);
    }

    [Fact]
    public void AppDomainTypeFinder_TypeExists()
    {
        var type = typeof(AppDomainTypeFinder);
        Assert.NotNull(type);
    }

    [Fact]
    public void IAssemblyFinder_TypeExists()
    {
        var type = typeof(IAssemblyFinder);
        Assert.NotNull(type);
    }

    [Fact]
    public void ITypeFinder_TypeExists()
    {
        var type = typeof(ITypeFinder);
        Assert.NotNull(type);
    }

    [Fact]
    public void ISession_TypeExists()
    {
        var type = typeof(ISession);
        Assert.NotNull(type);
    }

    [Fact]
    public void NullSession_TypeExists()
    {
        var type = typeof(NullSession);
        Assert.NotNull(type);
    }

    [Fact]
    public void DateTimeJsonConverter_TypeExists()
    {
        var type = typeof(DateTimeJsonConverter);
        Assert.NotNull(type);
    }

    [Fact]
    public void EnumJsonConverter_TypeExists()
    {
        var type = typeof(EnumJsonConverter<>);
        Assert.NotNull(type);
    }

    [Fact]
    public void EnumJsonConverterFactory_TypeExists()
    {
        var type = typeof(EnumJsonConverterFactory);
        Assert.NotNull(type);
    }

    [Fact]
    public void LongJsonConverter_TypeExists()
    {
        var type = typeof(LongJsonConverter);
        Assert.NotNull(type);
    }

    [Fact]
    public void NullableDateTimeJsonConverter_TypeExists()
    {
        var type = typeof(NullableDateTimeJsonConverter);
        Assert.NotNull(type);
    }

    [Fact]
    public void NullableLongJsonConverter_TypeExists()
    {
        var type = typeof(NullableLongJsonConverter);
        Assert.NotNull(type);
    }

    [Fact]
    public void UtcDateTimeJsonConverter_TypeExists()
    {
        var type = typeof(UtcDateTimeJsonConverter);
        Assert.NotNull(type);
    }

    [Fact]
    public void UtcNullableDateTimeJsonConverter_TypeExists()
    {
        var type = typeof(UtcNullableDateTimeJsonConverter);
        Assert.NotNull(type);
    }

    [Fact]
    public void HtmlAttribute_TypeExists()
    {
        var type = typeof(HtmlAttribute);
        Assert.NotNull(type);
    }

    [Fact]
    public void ModelAttribute_TypeExists()
    {
        var type = typeof(ModelAttribute);
        Assert.NotNull(type);
    }

    [Fact]
    public void DisposeAction_TypeExists()
    {
        var type = typeof(DisposeAction);
        Assert.NotNull(type);
    }

    [Fact]
    public void Item_TypeExists()
    {
        var type = typeof(Item);
        Assert.NotNull(type);
    }

    [Fact]
    public void JsonOptions_TypeExists()
    {
        var type = typeof(JsonOptions);
        Assert.NotNull(type);
    }

    [Fact]
    public void AutoMapperServiceRegistrar_TypeExists()
    {
        var type = typeof(AutoMapperServiceRegistrar);
        Assert.NotNull(type);
    }

    [Fact]
    public void IAutoMapperConfig_TypeExists()
    {
        var type = typeof(IAutoMapperConfig);
        Assert.NotNull(type);
    }

    [Fact]
    public void ObjectMapper_TypeExists()
    {
        var type = typeof(ObjectMapper);
        Assert.NotNull(type);
    }

    [Fact]
    public void IAopProxy_TypeExists()
    {
        var type = typeof(IAopProxy);
        Assert.NotNull(type);
    }

    [Fact]
    public void IgnoreAttribute_TypeExists()
    {
        var type = typeof(IgnoreAttribute);
        Assert.NotNull(type);
    }

    [Fact]
    public void NotEmptyAttribute_TypeExists()
    {
        var type = typeof(NotEmptyAttribute);
        Assert.NotNull(type);
    }

    [Fact]
    public void NotNullAttribute_TypeExists()
    {
        var type = typeof(NotNullAttribute);
        Assert.NotNull(type);
    }

    [Fact]
    public void IdCardAttribute_TypeExists()
    {
        var type = typeof(IdCardAttribute);
        Assert.NotNull(type);
    }

    [Fact]
    public void IValidation_TypeExists()
    {
        var type = typeof(IValidation);
        Assert.NotNull(type);
    }

    [Fact]
    public void IValidationHandler_TypeExists()
    {
        var type = typeof(IValidationHandler);
        Assert.NotNull(type);
    }

    [Fact]
    public void IValidationRule_TypeExists()
    {
        var type = typeof(IValidationRule);
        Assert.NotNull(type);
    }

    [Fact]
    public void NothingHandler_TypeExists()
    {
        var type = typeof(NothingHandler);
        Assert.NotNull(type);
    }

    [Fact]
    public void ThrowHandler_TypeExists()
    {
        var type = typeof(ThrowHandler);
        Assert.NotNull(type);
    }

    [Fact]
    public void ValidationResultCollection_TypeExists()
    {
        var type = typeof(ValidationResultCollection);
        Assert.NotNull(type);
    }

    [Fact]
    public void ValidAttribute_TypeExists()
    {
        var type = typeof(ValidAttribute);
        Assert.NotNull(type);
    }

    [Fact]
    public void UnauthenticatedIdentity_TypeExists()
    {
        var type = typeof(UnauthenticatedIdentity);
        Assert.NotNull(type);
    }

    [Fact]
    public void UnauthenticatedPrincipal_TypeExists()
    {
        var type = typeof(UnauthenticatedPrincipal);
        Assert.NotNull(type);
    }

    [Fact]
    public void DefaultPermissionManager_TypeExists()
    {
        var type = typeof(DefaultPermissionManager);
        Assert.NotNull(type);
    }

    [Fact]
    public void IPermissionManager_TypeExists()
    {
        var type = typeof(IPermissionManager);
        Assert.NotNull(type);
    }

    [Fact]
    public void IEncryptor_TypeExists()
    {
        var type = typeof(IEncryptor);
        Assert.NotNull(type);
    }

    [Fact]
    public void NullEncryptor_TypeExists()
    {
        var type = typeof(NullEncryptor);
        Assert.NotNull(type);
    }

    [Fact]
    public void IFilterOperation_TypeExists()
    {
        var type = typeof(IFilterOperation);
        Assert.NotNull(type);
    }

    [Fact]
    public void IPage_TypeExists()
    {
        var type = typeof(IPage);
        Assert.NotNull(type);
    }

    [Fact]
    public void ITrack_TypeExists()
    {
        var type = typeof(ITrack);
        Assert.NotNull(type);
    }

    [Fact]
    public void Pager_TypeExists()
    {
        var type = typeof(Pager);
        Assert.NotNull(type);
    }

    [Fact]
    public void QueryParameter_TypeExists()
    {
        var type = typeof(QueryParameter);
        Assert.NotNull(type);
    }

    [Fact]
    public void IQueryStore_TypeExists()
    {
        var type = typeof(IQueryStore<>);
        Assert.NotNull(type);
    }

    [Fact]
    public void IStore_TypeExists()
    {
        var type = typeof(IStore<>);
        Assert.NotNull(type);
    }

    [Fact]
    public void ITreeQueryParameter_TypeExists()
    {
        var type = typeof(ITreeQueryParameter);
        Assert.NotNull(type);
    }

    [Fact]
    public void TreeQueryParameter_TypeExists()
    {
        var type = typeof(TreeQueryParameter);
        Assert.NotNull(type);
    }

    [Fact]
    public void ConnectionStringCollection_TypeExists()
    {
        var type = typeof(ConnectionStringCollection);
        Assert.NotNull(type);
    }

    [Fact]
    public void ConnectionStringNameAttribute_TypeExists()
    {
        var type = typeof(ConnectionStringNameAttribute);
        Assert.NotNull(type);
    }

    [Fact]
    public void ICondition_TypeExists()
    {
        var type = typeof(ICondition<>);
        Assert.NotNull(type);
    }

    [Fact]
    public void IUnitOfWork_TypeExists()
    {
        var type = typeof(IUnitOfWork);
        Assert.NotNull(type);
    }

    [Fact]
    public void IUnitOfWorkActionManager_TypeExists()
    {
        var type = typeof(IUnitOfWorkActionManager);
        Assert.NotNull(type);
    }

    [Fact]
    public void NullUnitOfWorkActionManager_TypeExists()
    {
        var type = typeof(NullUnitOfWorkActionManager);
        Assert.NotNull(type);
    }

    [Fact]
    public void PageList_TypeExists()
    {
        var type = typeof(PageList<>);
        Assert.NotNull(type);
    }

    [Fact]
    public void FileData_TypeExists()
    {
        var type = typeof(FileData);
        Assert.NotNull(type);
    }

    [Fact]
    public void HttpClientService_TypeExists()
    {
        var type = typeof(HttpClientService);
        Assert.NotNull(type);
    }

    [Fact]
    public void HttpContentType_EnumExists()
    {
        var type = typeof(HttpContentType);
        Assert.NotNull(type);
        Assert.True(type.IsEnum);
    }

    [Fact]
    public void HttpRequest_TypeExists()
    {
        var type = typeof(HttpRequest<>);
        Assert.NotNull(type);
    }

    [Fact]
    public void IHttpClient_TypeExists()
    {
        var type = typeof(IHttpClient);
        Assert.NotNull(type);
    }

    [Fact]
    public void IHttpRequest_TypeExists()
    {
        var type = typeof(IHttpRequest);
        Assert.NotNull(type);
    }

    [Fact]
    public void IJsonSerializerOptionsFactory_TypeExists()
    {
        var type = typeof(IJsonSerializerOptionsFactory);
        Assert.NotNull(type);
    }

    [Fact]
    public void AspNetCoreServiceRegistrar_TypeExists()
    {
        var type = typeof(AspNetCoreServiceRegistrar);
        Assert.NotNull(type);
    }

    [Fact]
    public void AclFilter_TypeExists()
    {
        var type = typeof(AclFilter);
        Assert.NotNull(type);
    }

    [Fact]
    public void AclHandler_TypeExists()
    {
        var type = typeof(AclHandler);
        Assert.NotNull(type);
    }

    [Fact]
    public void AclMiddlewareResultHandler_TypeExists()
    {
        var type = typeof(AclMiddlewareResultHandler);
        Assert.NotNull(type);
    }

    [Fact]
    public void AclPolicyProvider_TypeExists()
    {
        var type = typeof(AclPolicyProvider);
        Assert.NotNull(type);
    }

    [Fact]
    public void AclRequirement_TypeExists()
    {
        var type = typeof(AclRequirement);
        Assert.NotNull(type);
    }
}