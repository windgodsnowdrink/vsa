#:sdk Microsoft.NET.Sdk.Web
#:sdk Aspire.AppHost.Sdk@9.4.2
#:package Aspire.Hosting.AppHost@9.4.2
#:package System.Threading.Channels@9.0.9
#:package Microsoft.Extensions.ObjectPool@9.0.9
#:package Scalar.AspNetCore@2.8.0
#:property LangVersion=preview
#:property TargetFramework=net11.0
#:property Nullable=enable
#:property ImplicitUsings=enable
#:property PublishAot=true

using System.Threading.Channels;
using Microsoft.Extensions.ObjectPool;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Scalar.AspNetCore;

file sealed class QueryFilterContext(DbContextOptions<QueryFilterContext> options) : DbContext(options)
{
    public DbSet<BlogPost> Posts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // �������ö�� query filter ��ָ������
        modelBuilder.Entity<BlogPost>()
            .HasQueryFilter("non-deleted", p => !p.Title.StartsWith("[Deleted]"))
            .HasQueryFilter("non-disabled", p => !p.Title.StartsWith("[Disabled]"))
            ;
        base.OnModelCreating(modelBuilder);
    }
}

public class BlogPost
{
    public int Id { get; set; }

    [StringLength(64)]
    public required string Title { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    [StringLength(64)]
    public required string UpdatedBy { get; set; }
}

const string connString = "DataSource=QueryFilterSample.db";
await using var services = new ServiceCollection()
        .AddLogging(lb => lb.AddDefaultDelegateLogger())
        .AddDbContext<QueryFilterContext>((provider, options) =>
        {
            options.UseSqlite(connString);
        })
        .BuildServiceProvider();

using var scope = services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<QueryFilterContext>();
await context.Database.EnsureDeletedAsync();
await context.Database.EnsureCreatedAsync();
{
    context.Posts.Add(new BlogPost
    {
        Title = "test",
        UpdatedAt = DateTimeOffset.Now,
        UpdatedBy = "test"
    });
    context.Posts.Add(new BlogPost
    {
        Title = "[Disabled]test",
        UpdatedAt = DateTimeOffset.Now,
        UpdatedBy = "test"
    });
    context.Posts.Add(new BlogPost
    {
        Title = "[Deleted]test",
        UpdatedAt = DateTimeOffset.Now,
        UpdatedBy = "test"
    });
    await context.SaveChangesAsync();
}

{
    var posts = await context.Posts.AsNoTracking().ToArrayAsync();
    Console.WriteLine(posts.Length);
    Console.WriteLine();
}

{
    var posts = await context.Posts.AsNoTracking()
        .IgnoreQueryFilters().ToArrayAsync();
    Console.WriteLine(posts.Length);
    Console.WriteLine();
}

{
    var posts = await context.Posts.AsNoTracking()
        .IgnoreQueryFilters(["non-deleted"]).ToArrayAsync();
    Console.WriteLine(posts.Length);
    Console.WriteLine();
}