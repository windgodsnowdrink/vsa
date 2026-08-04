const connection = new signalR.HubConnectionBuilder()
    .withUrl("/progressHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.on("ProgressUpdate", (progress) => {
    console.log(`Upload ${progress.UploadId}: ${progress.Percentage}%`);
    // 更新UI进度条
    document.getElementById('progress-bar').style.width = `${progress.Percentage}%`;
});

async function startUpload(file) {
    await connection.start();
    const formData = new FormData();
    formData.append('file', file);
    
    const response = await fetch('/api/minioupload/multipart-with-progress', {
        method: 'POST',
        body: formData
    });
    
    const result = await response.json();
    await connection.invoke("SubscribeToProgress", result.uploadId);
}