#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.SignalR.Client@8.0.0
#:package Microsoft.AspNetCore.Components.Web@8.0.0
#:property LangVersion preview
#:property TargetFramework net11.0
#:property Nullable enable

using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;

// SignalR Hub
public class TerminalHub : Hub
{
    public async Task SendCommand(string deviceId, string command)
    {
        await Clients.Group(deviceId).SendAsync("ReceiveCommand", command);
    }

    public async Task JoinDeviceGroup(string deviceId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, deviceId);
    }
}

// Blazor组件
public class TerminalComponent : ComponentBase
{
    [Parameter] public string DeviceId { get; set; }
    private HubConnection _hubConnection;
    private string _output = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("/terminalHub")
            .Build();

        _hubConnection.On<string>("ReceiveOutput", (output) =>
        {
            _output += output + "\n";
            StateHasChanged();
        });

        await _hubConnection.StartAsync();
        await _hubConnection.InvokeAsync("JoinDeviceGroup", DeviceId);
    }

    public async Task ExecuteCommand(string command)
    {
        await _hubConnection.InvokeAsync("SendCommand", DeviceId, command);
    }

    public void Dispose()
    {
        _hubConnection?.DisposeAsync();
    }
}