#:sdk Microsoft.NET.Sdk.Web
#:package Microsoft.AspNetCore.SignalR.Client@8.0.0
#:property LangVersion=preview
#:property TargetFramework=net10.0
#:property Nullable=enable

using Microsoft.AspNetCore.SignalR;

public class UploadProgressHub : Hub
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public async Task SubscribeToProgress(string uploadId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, uploadId);
    }
}

// 在Program.cs中添加
builder.Services.AddSignalR();
app.MapHub<UploadProgressHub>("/progressHub");