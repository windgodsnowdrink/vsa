#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package FastEndpoints@6.1.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable
#:property ImplicitUsings=enable

using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace FastEndpointsImpl
{
    /// <summary>
    /// 待办事项DTO
    /// </summary>
    public class TodoDto
    {
        /// <summary>
        /// 待办事项ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 待办事项标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsCompleted { get; set; }
    }

    /// <summary>
    /// 待办事项服务接口
    /// </summary>
    public interface ITodoService
    {
        /// <summary>
        /// 获取所有待办事项
        /// </summary>
        /// <returns>待办事项列表</returns>
        Task<IEnumerable<TodoDto>> GetAllAsync();

        /// <summary>
        /// 根据ID获取待办事项
        /// </summary>
        /// <param name="id">待办事项ID</param>
        /// <returns>待办事项</returns>
        Task<TodoDto> GetByIdAsync(int id);

        /// <summary>
        /// 创建待办事项
        /// </summary>
        /// <param name="dto">待办事项DTO</param>
        /// <returns>任务</returns>
        Task CreateAsync(TodoDto dto);

        /// <summary>
        /// 更新待办事项
        /// </summary>
        /// <param name="dto">待办事项DTO</param>
        /// <returns>任务</returns>
        Task UpdateAsync(TodoDto dto);

        /// <summary>
        /// 删除待办事项
        /// </summary>
        /// <param name="id">待办事项ID</param>
        /// <returns>任务</returns>
        Task DeleteAsync(int id);
    }

    /// <summary>
    /// FastEndpoints扩展
    /// </summary>
    public static class FastEndpointsExtensions
    {
        /// <summary>
        /// 实体名称
        /// </summary>
        private const string entityName = "Todo";

        /// <summary>
        /// 映射待办事项端点
        /// </summary>
        /// <param name="app">Web应用</param>
        public static void MapTodoEndpoints(this WebApplication app)
        {
            // 获取所有待办事项
            app.MapGet($"/api/{entityName.ToLower()}", 
                async (ITodoService service) => 
                Results.Ok(await service.GetAllAsync()))
                .WithTags($"{entityName}")
                .WithOpenApi();

            // 根据ID获取待办事项
            app.MapGet($"/api/{entityName.ToLower()}/{{id}}", 
                async (int id, ITodoService service) => 
                Results.Ok(await service.GetByIdAsync(id)))
                .WithTags($"{entityName}")
                .WithOpenApi();

            // 创建待办事项
            app.MapPost($"/api/{entityName.ToLower()}", 
                async (TodoDto dto, ITodoService service) => 
                {
                    await service.CreateAsync(dto);
                    return Results.Created($"/api/{entityName.ToLower()}/{dto.Id}", dto);
                })
                .WithTags($"{entityName}")
                .WithOpenApi();

            // 更新待办事项
            app.MapPut($"/api/{entityName.ToLower()}/{{id}}", 
                async (int id, TodoDto dto, ITodoService service) => 
                {
                    await service.UpdateAsync(dto);
                    return Results.NoContent();
                })
                .WithTags($"{entityName}")
                .WithOpenApi();

            // 删除待办事项
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
}