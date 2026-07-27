#!/usr/bin/env dotnet
#:sdk Microsoft.NET.Sdk.Web
#:package MediatR@12.1.1
#:property LangVersion=preview
#:property TargetFramework=net11.0

using System;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CqrsImplementation
{
    /// <summary>
    /// CQRS模式实现 - 查询部分
    /// </summary>
    
    /// <summary>
    /// 获取单个实体的查询
    /// </summary>
    /// <param name="Id">实体ID</param>
    public record Get{entityName}Query(int Id) : IRequest<{entityName}Dto>;
    
    /// <summary>
    /// 获取所有实体的查询
    /// </summary>
    public record GetAll{entityName}Query() : IRequest<IEnumerable<{entityName}Dto>>;
    
    /// <summary>
    /// 处理获取单个实体的查询处理器
    /// </summary>
    public class Get{entityName}QueryHandler : IRequestHandler<Get{entityName}Query, {entityName}Dto>
    {
        private readonly I{entityName}Repository _repository;
        private readonly IMapper _mapper;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">实体仓储</param>
        /// <param name="mapper">对象映射器</param>
        public Get{entityName}QueryHandler(
            I{entityName}Repository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        
        /// <summary>
        /// 处理查询请求
        /// </summary>
        /// <param name="request">查询请求</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>实体DTO</returns>
        public async Task<{entityName}Dto> Handle(
            Get{entityName}Query request, 
            CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            return _mapper.Map<{entityName}Dto>(entity);
        }
    }
    
    /// <summary>
    /// CQRS模式实现 - 命令部分
    /// </summary>
    
    /// <summary>
    /// 创建实体的命令
    /// </summary>
    /// <param name="Dto">创建实体的DTO</param>
    public record Create{entityName}Command({entityName}CreateDto Dto) : IRequest<int>;
    
    /// <summary>
    /// 更新实体的命令（包含版本控制）
    /// </summary>
    /// <param name="Dto">更新实体的DTO</param>
    /// <param name="Version">版本号，用于乐观并发控制</param>
    public record Update{entityName}Command({entityName}UpdateDto Dto, int Version) : IRequest;
    
    /// <summary>
    /// 删除实体的命令
    /// </summary>
    /// <param name="Id">实体ID</param>
    public record Delete{entityName}Command(int Id) : IRequest;
    
    /// <summary>
    /// 处理更新实体的命令处理器
    /// </summary>
    public class Update{entityName}CommandHandler : IRequestHandler<Update{entityName}Command>
    {
        private readonly I{entityName}Repository _repository;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">实体仓储</param>
        public Update{entityName}CommandHandler(I{entityName}Repository repository)
        {
            _repository = repository;
        }
        
        /// <summary>
        /// 处理更新命令
        /// </summary>
        /// <param name="request">更新命令</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        public async Task Handle(
            Update{entityName}Command request, 
            CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Dto.Id);
            
            // 乐观并发检查
            if (entity.Version != request.Version)
                throw new ConcurrencyConflictException();
            
            // 更新实体和版本
            entity.Version++;
            await _repository.UpdateAsync(entity);
        }
    }
    
    /// <summary>
    /// 处理创建实体的命令处理器
    /// </summary>
    public class Create{entityName}CommandHandler : IRequestHandler<Create{entityName}Command, int>
    {
        private readonly I{entityName}Repository _repository;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">实体仓储</param>
        public Create{entityName}CommandHandler(I{entityName}Repository repository)
        {
            _repository = repository;
        }
        
        /// <summary>
        /// 处理创建命令
        /// </summary>
        /// <param name="request">创建命令</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>创建的实体ID</returns>
        public async Task<int> Handle(
            Create{entityName}Command request, 
            CancellationToken cancellationToken)
        {
            var entity = new {entityName}();
            // 映射属性
            await _repository.AddAsync(entity);
            return entity.Id;
        }
    }
    
    /// <summary>
    /// 处理删除实体的命令处理器
    /// </summary>
    public class Delete{entityName}CommandHandler : IRequestHandler<Delete{entityName}Command>
    {
        private readonly I{entityName}Repository _repository;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="repository">实体仓储</param>
        public Delete{entityName}CommandHandler(I{entityName}Repository repository)
        {
            _repository = repository;
        }
        
        /// <summary>
        /// 处理删除命令
        /// </summary>
        /// <param name="request">删除命令</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns>任务</returns>
        public async Task Handle(
            Delete{entityName}Command request, 
            CancellationToken cancellationToken)
        {
            await _repository.DeleteAsync(request.Id);
        }
    }
    
    /// <summary>
    /// 并发冲突异常
    /// </summary>
    public class ConcurrencyConflictException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ConcurrencyConflictException()
            : base("并发冲突：实体已被其他用户修改")
        {}
    }
    
    /// <summary>
    /// 实体仓储接口
    /// </summary>
    public interface I{entityName}Repository
    {
        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>实体</returns>
        Task<{entityName}> GetByIdAsync(int id);
        
        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <returns>实体集合</returns>
        Task<IEnumerable<{entityName}>> GetAllAsync();
        
        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>任务</returns>
        Task AddAsync({entityName} entity);
        
        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>任务</returns>
        Task UpdateAsync({entityName} entity);
        
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id">实体ID</param>
        /// <returns>任务</returns>
        Task DeleteAsync(int id);
    }
    
    /// <summary>
    /// 实体类
    /// </summary>
    public class {entityName}
    {
        /// <summary>
        /// 实体ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 版本号，用于乐观并发控制
        /// </summary>
        public int Version { get; set; }
        
        // 其他实体属性
    }
    
    /// <summary>
    /// 实体DTO
    /// </summary>
    public class {entityName}Dto
    {
        /// <summary>
        /// 实体ID
        /// </summary>
        public int Id { get; set; }
        
        // 其他DTO属性
    }
    
    /// <summary>
    /// 创建实体的DTO
    /// </summary>
    public class {entityName}CreateDto
    {
        // 创建DTO属性
    }
    
    /// <summary>
    /// 更新实体的DTO
    /// </summary>
    public class {entityName}UpdateDto
    {
        /// <summary>
        /// 实体ID
        /// </summary>
        public int Id { get; set; }
        
        // 其他更新DTO属性
    }
    
    /// <summary>
    /// 对象映射器接口
    /// </summary>
    public interface IMapper
    {
        /// <summary>
        /// 映射对象
        /// </summary>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <returns>映射后的目标对象</returns>
        TDestination Map<TDestination>(object source);
    }
}

