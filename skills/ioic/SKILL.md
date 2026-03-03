# ioic Agent Skill - ioic Skill

## Skill Overview

High performance ioic skill based on .NET 10, providing powerful ioic functionality for .NET developers.

## Quick Start Guide

### Install Dependencies

Add the following dependencies to your main application's runfile:

`yaml
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Logging@10.0.0
`

### Register Services

Register ioic services in your main application:

`csharp
// Register ioic services
builder.Services.AddSingleton<>();
builder.Services.AddSingleton<I, >();
`

### Usage Example

`csharp
// Get ioic service
var  = serviceProvider.GetRequiredService<>();

// Use ioic functionality
var result = await .DoSomethingAsync();
Console.WriteLine($"Result: {result}");
`

## Navigation Map

`
ioic/
????? index.yaml                   # Metadata index description
????? SKILL.md                    # Skill entry point (current file)
????? reference/                  # Reference files
??  ????? README.md              # Complete feature description
??  ????? examples.md            # Usage examples
????? scripts/                    # Scripts and tools
    ????? .cs     # ioic core implementation
    ????? .run.json  # Run configuration
    ????? .setting.json  # Setting file
`

## Main Features

1. **Core Feature 1**: ioic core feature description
2. **Core Feature 2**: ioic core feature description
3. **Core Feature 3**: ioic core feature description
4. **High Performance Design**: Optimized performance implementation
5. **Easy-to-use API**: Simple and intuitive API design
6. **Extensible Architecture**: Support for custom extensions

## Extension Instructions

This skill provides a complete ioic solution that you can extend as needed:

1. **Custom Implementation**: Implement the I interface
2. **Extend Features**: Add new ioic features
3. **Integrate with Other Systems**: Integrate with other systems
4. **Performance Optimization**: Optimize performance for specific scenarios

## Best Practices

1. **Dependency Injection**: Use dependency injection to manage services
2. **Async Programming**: Prefer async APIs to avoid blocking
3. **Error Handling**: Properly handle exception cases
4. **Logging**: Add appropriate logging
5. **Performance Monitoring**: Monitor key performance indicators
