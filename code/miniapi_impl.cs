#:sdk Microsoft.NET.Sdk.Web
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;

public static class MinimalApiExtensions
{
    public static void Map{entityName}MinimalEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/{entityName.ToLower()}")
            .WithTags("{entityName}")
            .WithOpenApi();

        group.MapGet("", async (I{entityName}Service service) => 
            await service.GetAllAsync());

        group.MapGet("/{id}", async (int id, I{entityName}Service service) => 
            await service.GetByIdAsync(id));

        group.MapPost("", async ({entityName}Dto dto, I{entityName}Service service) => 
        {
            await service.CreateAsync(dto);
            return Results.Created($"/api/{entityName.ToLower()}/{dto.Id}", dto);
        });

        group.MapPut("/{id}", async (int id, {entityName}Dto dto, I{entityName}Service service) => 
        {
            await service.UpdateAsync(dto);
            return Results.NoContent();
        });

        group.MapDelete("/{id}", async (int id, I{entityName}Service service) => 
        {
            await service.DeleteAsync(id);
            return Results.NoContent();
        });
    }
}