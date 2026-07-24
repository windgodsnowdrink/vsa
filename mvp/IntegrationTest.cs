#load "Integration.cs"

Console.WriteLine("=== Integration (Eshop) Test ===");

try
{
    var programType = typeof(App.Program);
    Console.WriteLine("[PASS] Program type found: " + programType.FullName);

    var iTransientType = typeof(App.ITransientService);
    Console.WriteLine("[PASS] ITransientService interface found: " + iTransientType.FullName);
    var iScopedType = typeof(App.IScopedService);
    Console.WriteLine("[PASS] IScopedService interface found: " + iScopedType.FullName);
    var iSingletonType = typeof(App.ISingletonService);
    Console.WriteLine("[PASS] ISingletonService interface found: " + iSingletonType.FullName);
    var iOpenGenericType = typeof(App.IOpenGeneric<>);
    Console.WriteLine("[PASS] IOpenGeneric<T> interface found: " + iOpenGenericType.FullName);
    var iMyServiceType = typeof(App.IMyService);
    Console.WriteLine("[PASS] IMyService interface found: " + iMyServiceType.FullName);
    var myServiceType = typeof(App.MyService);
    Console.WriteLine("[PASS] MyService class found: " + myServiceType.FullName);
    var loggingDecoratorType = typeof(App.LoggingDecorator);
    Console.WriteLine("[PASS] LoggingDecorator class found: " + loggingDecoratorType.FullName);

    var catalogServiceExtensionsType = typeof(App.Modules.Catalog.ServiceCollectionExtensions);
    Console.WriteLine("[PASS] Catalog.ServiceCollectionExtensions found: " + catalogServiceExtensionsType.FullName);
    var productDtoType = typeof(App.Modules.Catalog.ProductDto);
    Console.WriteLine("[PASS] ProductDto found: " + productDtoType.FullName);
    var productType = typeof(App.Modules.Catalog.Product);
    Console.WriteLine("[PASS] Product class found: " + productType.FullName);
    var catalogDbContextType = typeof(App.Modules.Catalog.CatalogDbContext);
    Console.WriteLine("[PASS] CatalogDbContext found: " + catalogDbContextType.FullName);

    var basketServiceExtensionsType = typeof(App.Modules.Basket.ServiceCollectionExtensions);
    Console.WriteLine("[PASS] Basket.ServiceCollectionExtensions found: " + basketServiceExtensionsType.FullName);
    var shoppingCartType = typeof(App.Modules.Basket.ShoppingCart);
    Console.WriteLine("[PASS] ShoppingCart class found: " + shoppingCartType.FullName);
    var shoppingCartItemType = typeof(App.Modules.Basket.ShoppingCartItem);
    Console.WriteLine("[PASS] ShoppingCartItem class found: " + shoppingCartItemType.FullName);
    var iBasketRepoType = typeof(App.Modules.Basket.IBasketRepository);
    Console.WriteLine("[PASS] IBasketRepository interface found: " + iBasketRepoType.FullName);

    var orderingServiceExtensionsType = typeof(App.Modules.Ordering.ServiceCollectionExtensions);
    Console.WriteLine("[PASS] Ordering.ServiceCollectionExtensions found: " + orderingServiceExtensionsType.FullName);
    var orderType = typeof(App.Modules.Ordering.Order);
    Console.WriteLine("[PASS] Order class found: " + orderType.FullName);
    var orderItemType = typeof(App.Modules.Ordering.OrderItem);
    Console.WriteLine("[PASS] OrderItem class found: " + orderItemType.FullName);

    var aggregateType = typeof(App.Shared.Aggregate<object>);
    Console.WriteLine("[PASS] Aggregate<T> class found: " + aggregateType.FullName);
    var entityType = typeof(App.Shared.Entity<object>);
    Console.WriteLine("[PASS] Entity<T> class found: " + entityType.FullName);
    var iDomainEventType = typeof(App.Shared.IDomainEvent);
    Console.WriteLine("[PASS] IDomainEvent interface found: " + iDomainEventType.FullName);
    var integrationEventType = typeof(App.Shared.IntegrationEvent);
    Console.WriteLine("[PASS] IntegrationEvent found: " + integrationEventType.FullName);
    var paginatedResultType = typeof(App.Shared.PaginatedResult<object>);
    Console.WriteLine("[PASS] PaginatedResult<T> found: " + paginatedResultType.FullName);

    Console.WriteLine("=== 测试完成 ===");
}
catch (Exception ex)
{
    Console.WriteLine($"[FAIL] 测试失败: {ex.Message}");
}