#:sdk Microsoft.NET.Sdk
#:package MediatR@12.1.1
#:property LangVersion preview
#:property TargetFramework net11.0

// 查询实现
public record Get{entityName}Query(int Id) : IRequest<{entityName}Dto>;
public record GetAll{entityName}Query() : IRequest<IEnumerable<{entityName}Dto>>;

public class Get{entityName}QueryHandler : IRequestHandler<Get{entityName}Query, {entityName}Dto>
{
    private readonly I{entityName}Repository _repository;
    private readonly IMapper _mapper;

    public Get{entityName}QueryHandler(
        I{entityName}Repository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<{entityName}Dto> Handle(
        Get{entityName}Query request, 
        CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id);
        return _mapper.Map<{entityName}Dto>(entity);
    }
}

// 命令实现
public record Create{entityName}Command({entityName}CreateDto Dto) : IRequest<int>;
// 在命令中添加版本控制
public record Update{entityName}Command({entityName}UpdateDto Dto, int Version) : IRequest;

public class Update{entityName}CommandHandler : IRequestHandler<Update{entityName}Command>
{
    private readonly I{entityName}Repository _repository;

    public Update{entityName}CommandHandler(I{entityName}Repository repository)
    {
        _repository = repository;
    }

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

public record Delete{entityName}Command(int Id) : IRequest;

public class Create{entityName}CommandHandler : IRequestHandler<Create{entityName}Command, int>
{
    private readonly I{entityName}Repository _repository;

    public Create{entityName}CommandHandler(I{entityName}Repository repository)
    {
        _repository = repository;
    }

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