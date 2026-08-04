#:sdk Microsoft.NET.Sdk.Web
#:package FastEndpoints@6.1.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using FastEndpoints;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;

public static class FastEndpointsExtensions
{
    string entityName = "Todo";

    public static void MapTodoEndpoints(this WebApplication app)
    {
        app.MapGet($"/api/{entityName.ToLower()}", 
            async (ITodoService service) => 
            Results.Ok(await service.GetAllAsync()))
            .WithTags($"{entityName}")
            .WithOpenApi();

        app.MapGet($"/api/{entityName.ToLower()}/{{id}}", 
            async (int id, ITodoService service) => 
            Results.Ok(await service.GetByIdAsync(id)))
            .WithTags($"{entityName}")
            .WithOpenApi();

        app.MapPost($"/api/{entityName.ToLower()}", 
            async (TodoDto dto, ITodoService service) => 
            {
                await service.CreateAsync(dto);
                return Results.Created($"/api/{entityName.ToLower()}/{dto.Id}", dto);
            })
            .WithTags($"{entityName}")
            .WithOpenApi();

        app.MapPut($"/api/{entityName.ToLower()}/{{id}}", 
            async (int id, TodoDto dto, ITodoService service) => 
            {
                await service.UpdateAsync(dto);
                return Results.NoContent();
            })
            .WithTags($"{entityName}")
            .WithOpenApi();

        app.MapDelete($"/api/{entityName.ToLower()}/{{id}}", 
            async (int id, ITodoService service) => 
            {
                await service.DeleteAsync(id);
                return Results.NoContent();
            })
            .WithTags($"{entityName}")
            .WithOpenApi();
    }
}