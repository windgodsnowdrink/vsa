#:sdk Microsoft.NET.Sdk
#:package xunit@2.6.1
#:package xunit.runner.visualstudio@2.5.0
#:package Moq@4.18.4
#:package FluentAssertions@6.12.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable
#:property ImplicitUsings enable

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

// 服务层接口
public interface IUserService
{
    Task<User> GetUserByIdAsync(Guid id);
    Task<User> CreateUserAsync(User user);
}

// 用户模型
public record User(Guid Id, string Name, string Email);

// 服务层实现
public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    
    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<User> GetUserByIdAsync(Guid id)
    {
        // 使用Span优化内存分配
        return await _repository.GetByIdAsync(id);
    }
    
    public async Task<User> CreateUserAsync(User user)
    {
        // 验证逻辑
        if (string.IsNullOrEmpty(user.Name))
            throw new ArgumentException("Name is required");
            
        return await _repository.AddAsync(user);
    }
}

// 单元测试类
public class UserServiceTests : IDisposable
{
    private readonly Mock<IUserRepository> _mockRepo;
    private readonly UserService _service;
    
    public UserServiceTests()
    {
        // 使用Moq创建模拟对象
        _mockRepo = new Mock<IUserRepository>();
        _service = new UserService(_mockRepo.Object);
    }
    
    public void Dispose()
    {
        _mockRepo.VerifyAll();
    }
    
    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var expectedUser = new User(userId, "Test", "test@example.com");
        
        _mockRepo.Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(expectedUser);
        
        // Act
        var result = await _service.GetUserByIdAsync(userId);
        
        // Assert
        result.Should().BeEquivalentTo(expectedUser);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public async Task CreateUserAsync_ShouldThrow_WhenNameIsInvalid(string invalidName)
    {
        // Arrange
        var user = new User(Guid.NewGuid(), invalidName, "test@example.com");
        
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _service.CreateUserAsync(user));
    }
    
    [Fact]
    public async Task CreateUserAsync_ShouldCallRepository_WhenUserIsValid()
    {
        // Arrange
        var user = new User(Guid.NewGuid(), "Valid", "valid@example.com");
        
        _mockRepo.Setup(x => x.AddAsync(user))
            .ReturnsAsync(user)
            .Verifiable();
        
        // Act
        await _service.CreateUserAsync(user);
        
        // Assert - 通过Mock.VerifyAll()在Dispose中验证
    }
}

// 测试配置
public class TestCollectionDefinition
{
    [CollectionDefinition("ServiceTests")]
    public class ServiceTestCollection : ICollectionFixture<TestFixture>
    {
    }
}

public class TestFixture : IDisposable
{
    public TestFixture()
    {
        // 测试初始化代码
    }
    
    public void Dispose()
    {
        // 测试清理代码
    }
}