#:sdk Microsoft.NET.Sdk.Web
#:package RestSharp@110.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using RestSharp;
using System.Threading.Channels;

public interface IUserService
{
    [Get("/api/users/{id}")]
    Task<User> GetUserAsync(int id, CancellationToken ct = default);
    
    [Post("/api/users")]
    Task<User> CreateUserAsync([Body] User user, CancellationToken ct = default);
    
    [Put("/api/users/{id}")]
    Task<User> UpdateUserAsync(int id, [Body] User user, CancellationToken ct = default);
    
    [Delete("/api/users/{id}")]
    Task<bool> DeleteUserAsync(int id, CancellationToken ct = default);
}

public record User(int Id, string Name, string Email);