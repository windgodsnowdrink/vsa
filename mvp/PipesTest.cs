#load "Pipes.cs"

using System.Reflection;

public class PipesTest
{
    [Fact]
    public void PipeMessage_TypeExists()
    {
        var type = typeof(PipeMessage);
        Assert.NotNull(type);
        var idProp = type.GetProperty("Id");
        Assert.NotNull(idProp);
        var messageTypeProp = type.GetProperty("MessageType");
        Assert.NotNull(messageTypeProp);
        var timestampProp = type.GetProperty("Timestamp");
        Assert.NotNull(timestampProp);
        var dataProp = type.GetProperty("Data");
        Assert.NotNull(dataProp);
        var metadataProp = type.GetProperty("Metadata");
        Assert.NotNull(metadataProp);
    }

    [Fact]
    public void PipeSecurityManager_TypeExists()
    {
        var type = typeof(PipeSecurityManager);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("ValidateUserAccess"));
        Assert.NotNull(type.GetMethod("GetCurrentUserName"));
        Assert.NotNull(type.GetMethod("AddAllowedUser"));
        Assert.NotNull(type.GetMethod("AddBlockedUser"));
        Assert.NotNull(type.GetMethod("GetSecurityUsersInfo"));
    }

    [Fact]
    public void NamedPipeClientService_TypeExists()
    {
        var type = typeof(NamedPipeClientService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("SendMessageAsync"));
        Assert.NotNull(type.GetMethod("SendBatchMessagesAsync"));
    }

    [Fact]
    public void NamedPipeServerService_TypeExists()
    {
        var type = typeof(NamedPipeServerService);
        Assert.NotNull(type);
        Assert.NotNull(type.GetMethod("StartServerAsync"));
        Assert.NotNull(type.GetMethod("StopServerAsync"));
    }
}