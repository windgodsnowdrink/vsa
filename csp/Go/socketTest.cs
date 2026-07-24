#load "socket.cs"

using System.Reflection;

public class socketTest
{
    [Fact]
    public void socket_result_TypeExists()
    {
        var type = typeof(socket_result);
        Assert.NotNull(type);
    }

    [Fact]
    public void socket_tcp_TypeExists()
    {
        var type = typeof(socket_tcp);
        Assert.NotNull(type);
    }

    [Fact]
    public void socket_serial_TypeExists()
    {
        var type = typeof(socket_serial);
        Assert.NotNull(type);
    }

    [Fact]
    public void socket_pipe_server_TypeExists()
    {
        var type = typeof(socket_pipe_server);
        Assert.NotNull(type);
    }

    [Fact]
    public void socket_pipe_client_TypeExists()
    {
        var type = typeof(socket_pipe_client);
        Assert.NotNull(type);
    }
}